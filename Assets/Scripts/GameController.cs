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
	public static Action OnSnakeDestroyed;

	[SerializeField] private Button startGameButton;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button quitButton;
	[SerializeField] private GameObject startGamePanel;
	[SerializeField] private GameObject gameOverPanel;
	
	public static bool PlayerInputReceived;
	public static bool IsTurnFinished = true;
	public static int CurrentStage { get; private set; } = 1;

	public static int RandomSeed { get; private set; }
	public int StartingPopulation;
	private static World HamsterWorld;
	public static PlayerInputSettings PlayerInputSettings { get; private set;}

	private void Awake()
	{
		HamsterWorld = World.DefaultGameObjectInjectionWorld;
		startGameButton.onClick.AddListener(StartGame);
		restartButton.onClick.AddListener(ResetGame);
		quitButton.onClick.AddListener(QuitGame);
		CompleteStageSystem.OnStageComplete += NextStage;
		GameOverSystem.OnGameOver += GameOver;
		PlayerInputSettings = new PlayerInputSettings();
		PlayerInputSettings.Application.Quit.performed += QuitGame;
	}

	private void StartGame()
	{
		StartCoroutine(SetUpGame());
	}

	private void GameOver()
	{
		gameOverPanel.SetActive(true);
	}

	private void OnDestroy()
	{
		startGameButton.onClick.RemoveAllListeners();
		CompleteStageSystem.OnStageComplete -= NextStage;
		GameOverSystem.OnGameOver -= GameOver;
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
		QuitGame();
	}
	
	private void QuitGame()
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
		
		var stageSpawn = HamsterWorld.GetOrCreateSystem(typeof(StageSpawnSystem));
		stageSpawn.Update(HamsterWorld.Unmanaged);
		
		var botSpawn = HamsterWorld.GetOrCreateSystem(typeof(BotSpawnSystem));
		botSpawn.Update(HamsterWorld.Unmanaged);
		
		OnStageChanged?.Invoke(CurrentStage);
		OnPopulationChanged.Invoke(PopulationSystem.Population);
	}

	private static void SetSystems()
	{
		var initializationSystemGroup = HamsterWorld.GetOrCreateSystemManaged<InitializationSystemGroup>();
		var snakeSpawn = HamsterWorld.GetOrCreateSystem(typeof(SnakeSpawnSystem));
		initializationSystemGroup.AddSystemToUpdateList(snakeSpawn);
		
		var simulationSystemGroup = HamsterWorld.GetOrCreateSystemManaged<SimulationSystemGroup>();
		
		var inputSystem = HamsterWorld.GetOrCreateSystem(typeof(InputDetectionSystem));
		simulationSystemGroup.AddSystemToUpdateList(inputSystem);
		
		var botDecision = HamsterWorld.GetOrCreateSystem(typeof(BotDecisionSystem));
		simulationSystemGroup.AddSystemToUpdateList(botDecision);
		
		var snakeDecision = HamsterWorld.GetOrCreateSystem(typeof(SnakeDecisionSystem));
		simulationSystemGroup.AddSystemToUpdateList(snakeDecision);
		
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
		
		var gameOverSystem = HamsterWorld.GetOrCreateSystem(typeof(GameOverSystem));
		lateSystemGroup.AddSystemToUpdateList(gameOverSystem);
		
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
		ResetStage();
	}

	private void ResetStage()
	{
		TilesSpawnSystem.ResetTiles();
		
		DestroyBots();
		DestroySnake();

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

	private void DestroyBots()
	{
		HamsterWorld.EntityManager.CompleteAllTrackedJobs();
		var botQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<BotComponent>().Build(HamsterWorld.EntityManager);
		var entities = botQuery.ToEntityArray(Allocator.TempJob);
		HamsterWorld.EntityManager.DestroyEntity(entities);
		entities.Dispose();
	}

	private void DestroySnake()
	{
		var snakeQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<SnakeTag>().Build(HamsterWorld.EntityManager);
		var entities = snakeQuery.ToEntityArray(Allocator.TempJob);
		HamsterWorld.EntityManager.DestroyEntity(entities);
		entities.Dispose();
		OnSnakeDestroyed?.Invoke();
	}

	private void ResetGame()
	{
		CurrentStage = 1;
		PopulationSystem.SetStartingPopulation(StartingPopulation);
		ResetStage();
		gameOverPanel.SetActive(false);
	}
}
