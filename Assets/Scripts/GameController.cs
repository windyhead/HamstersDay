using System;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : SingletonBehaviour
{
	public static Action<int> OnStageChanged;
	
	public static Action<int> OnPopulationChanged;
	
	public static World HamsterWorld { get; private set; }

	public static bool PlayerInputReceived;
	
	public static bool IsTurnFinished = true;

	private static int CurrentStage = 1;
	
	public static int RandomSeed { get; private set; }

	public int StartingPopulation;

	private void Awake()
	{
		HamsterWorld = World.DefaultGameObjectInjectionWorld;
		CompleteStageSystem.OnStageComplete += NextStage;
	}
	
	private void Start()
	{
		StartCoroutine(StartGame());
	}

	private void OnDestroy()
	{
		CompleteStageSystem.OnStageComplete -= NextStage;
		World.DisposeAllWorlds();
	}

	private IEnumerator StartGame()
	{
		SceneManager.LoadSceneAsync("MainScene",LoadSceneMode.Additive);
		yield return new WaitForSeconds(1);
		CurrentStage = 1;
		PopulationSystem.SetStartingPopulation(StartingPopulation);
		SetStage();
		SetSystems();
		TurnSystem.ResetTimer();
		CompleteStageSystem.ResetStage();
		var complete = HamsterWorld.GetExistingSystem<CompleteStageSystem>();
		SystemState state = HamsterWorld.Unmanaged.ResolveSystemStateRef(complete);
		state.Enabled = true;
	}
	
	private static void SetStage()
	{
		RandomSeed = Random.Range(1, 101);
		
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
		
		var transformSystemGroup = HamsterWorld.GetOrCreateSystemManaged<TransformSystemGroup>();
		
		var moveSystem = HamsterWorld.GetOrCreateSystem(typeof(MoveSystem));
		transformSystemGroup.AddSystemToUpdateList(moveSystem);
		
		var lateSystemGroup = HamsterWorld.GetOrCreateSystemManaged<LateSimulationSystemGroup>();
		
		var endTurn= HamsterWorld.GetOrCreateSystem(typeof(TurnSystem));
		lateSystemGroup.AddSystemToUpdateList(endTurn);
		
		var populaton= HamsterWorld.GetOrCreateSystem(typeof(PopulationSystem));
		lateSystemGroup.AddSystemToUpdateList(populaton);
		
		var stageComplete= HamsterWorld.GetOrCreateSystem(typeof(CompleteStageSystem));
		lateSystemGroup.AddSystemToUpdateList(stageComplete);
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
