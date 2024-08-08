using System;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : SingletonBehaviour
{
	public static Action<int> OnStageChanged;
	
	public static Action<int> OnPopulationChanged;

	[SerializeField] private Button startButton;

	[SerializeField] private GameObject startGamePanel;
	
	public static bool PlayerInputReceived;
	
	public static bool IsTurnFinished = true;

	private static int CurrentStage = 1;
	
	public static int RandomSeed { get; private set; }

	public int StartingPopulation;
	
	private static World HamsterWorld;

	public static PlayerInputSettings PlayerInputSettings { get; private set;}

	private void Awake()
	{
		HamsterWorld = World.DefaultGameObjectInjectionWorld;
		startButton.onClick.AddListener(StartGame);
		CompleteStageSystem.OnStageComplete += NextStage;
		PlayerInputSettings = new PlayerInputSettings();
		PlayerInputSettings.Application.Quit.performed += QuitGame;
	}
	
	private void StartGame()
	{
		StartCoroutine(SetUpGame());
	}

	private void OnDestroy()
	{
		startButton.onClick.RemoveAllListeners();
		CompleteStageSystem.OnStageComplete -= NextStage;
		PlayerInputSettings.Application.Quit.performed -= QuitGame;
		World.DisposeAllWorlds();
	}

	private IEnumerator SetUpGame()
	{
		SceneManager.LoadSceneAsync("MainScene",LoadSceneMode.Additive);
		yield return new WaitForSeconds(1);
		startGamePanel.SetActive(false);
		PopulationSystem.SetStartingPopulation(StartingPopulation);
		SetStage();
		SetSystems();
	}

	private void QuitGame(InputAction.CallbackContext callbackContext)
	{
#if UNITY_EDITOR
		
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	private static void SetStage()
	{
		RandomSeed = Random.Range(1, 101);
		CurrentStage = 1;
		
		var tileSpawn = HamsterWorld.GetOrCreateSystem(typeof(TilesSpawnSystem));
		tileSpawn.Update(HamsterWorld.Unmanaged);
		
		var playerSpawn = HamsterWorld.GetOrCreateSystem(typeof(PlayerSpawnSystem));
		playerSpawn.Update(HamsterWorld.Unmanaged);
		
		var wheelSpawn = HamsterWorld.GetOrCreateSystem(typeof(HouseSpawnSystem));
		wheelSpawn.Update(HamsterWorld.Unmanaged);
		
		var botSpawn = HamsterWorld.GetOrCreateSystem(typeof(BotSpawnSystem));
		botSpawn.Update(HamsterWorld.Unmanaged);
		OnStageChanged?.Invoke(CurrentStage);
		OnPopulationChanged.Invoke(PopulationSystem.Population);
	}

	private static void SetSystems()
	{
		var simulationSystemGroup = HamsterWorld.GetOrCreateSystemManaged<SimulationSystemGroup>();
		
		var inputSystem = HamsterWorld.GetOrCreateSystem(typeof(InputDetectionSystem));
		simulationSystemGroup.AddSystemToUpdateList(inputSystem);
		
		var aiSystem = HamsterWorld.GetOrCreateSystem(typeof(BotDecisionSystem));
		simulationSystemGroup.AddSystemToUpdateList(aiSystem);
		
		var orientationSystem = HamsterWorld.GetOrCreateSystem(typeof(OrientationSystem));
		simulationSystemGroup.AddSystemToUpdateList(orientationSystem);
		
		var destroy= HamsterWorld.GetOrCreateSystem(typeof(BotDestroySystem));
		simulationSystemGroup.AddSystemToUpdateList(destroy);
		
		var transformSystemGroup = HamsterWorld.GetOrCreateSystemManaged<TransformSystemGroup>();
		
		var moveSystem = HamsterWorld.GetOrCreateSystem(typeof(MoveSystem));
		transformSystemGroup.AddSystemToUpdateList(moveSystem);
		
		var rotationSystem = HamsterWorld.GetOrCreateSystem(typeof(RotationSystem));
		transformSystemGroup.AddSystemToUpdateList(rotationSystem);
		
		var lateSystemGroup = HamsterWorld.GetOrCreateSystemManaged<LateSimulationSystemGroup>();
		
		var endTurn= HamsterWorld.GetOrCreateSystem(typeof(TurnSystem));
		lateSystemGroup.AddSystemToUpdateList(endTurn);
		TurnSystem.ResetTimer();
		
		var population= HamsterWorld.GetOrCreateSystem(typeof(PopulationSystem));
		lateSystemGroup.AddSystemToUpdateList(population);
		
		
		var stageComplete= HamsterWorld.GetOrCreateSystem(typeof(CompleteStageSystem));
		lateSystemGroup.AddSystemToUpdateList(stageComplete);
		
		SystemState state = HamsterWorld.Unmanaged.ResolveSystemStateRef(stageComplete);
		CompleteStageSystem.ResetStage();
		state.Enabled = true;
	}

	private void NextStage()
	{
		CurrentStage ++;
		TilesSpawnSystem.ResetTiles();
		
		HamsterWorld.EntityManager.CompleteAllTrackedJobs();
		var botQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<BotComponent>().Build(HamsterWorld.EntityManager);
		var entities = botQuery.ToEntityArray(Allocator.TempJob);
		HamsterWorld.EntityManager.DestroyEntity(entities);
		entities.Dispose();
		
		var playerReset = HamsterWorld.GetOrCreateSystem(typeof(PlayerResetSystem));
		playerReset.Update(HamsterWorld.Unmanaged);
		
		var botSpawn = HamsterWorld.GetOrCreateSystem(typeof(BotSpawnSystem));
		botSpawn.Update(HamsterWorld.Unmanaged);

		 var complete = HamsterWorld.GetExistingSystem<CompleteStageSystem>();
		 ref SystemState state = ref HamsterWorld.Unmanaged.ResolveSystemStateRef(complete);
		 state.Enabled = true;
		 
		 TurnSystem.ResetTimer();
		 OnStageChanged?.Invoke(CurrentStage);
		 OnPopulationChanged?.Invoke(PopulationSystem.Population);
	}
}
