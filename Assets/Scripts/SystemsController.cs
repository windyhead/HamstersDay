using Unity.Entities;
using Unity.Transforms;
using Random = UnityEngine.Random;

public class SystemsController : SingletonBehaviour<SystemsController>
{
	public static int RandomSeed { get; private set; }
	private static World HamsterWorld;
	
	private void Awake()
	{
		GameController.OnEntitySceneLoaded += SetWorld;
		GameController.OnStageChanged += ResetStage;
		GameController.OnGameReset += ResetGame;
	}
	
	private void OnDestroy()
	{
		GameController.OnEntitySceneLoaded -= SetWorld;
		GameController.OnStageChanged -= ResetStage;
		GameController.OnGameReset -= ResetGame;
		World.DisposeAllWorlds();
	}

	private void SetWorld()
	{
		HamsterWorld = World.DefaultGameObjectInjectionWorld;
		PopulationSystem.SetStartingPopulation();
		SetStage();
		SetSystems();
	}

	private void SetStage()
	{
		RandomSeed = Random.Range(1, 101);
		
		var tileSpawn = HamsterWorld.GetOrCreateSystem(typeof(TilesSpawnSystem));
		tileSpawn.Update(HamsterWorld.Unmanaged);
		
		var playerSpawn = HamsterWorld.GetOrCreateSystem(typeof(PlayerSpawnSystem));
		playerSpawn.Update(HamsterWorld.Unmanaged);
		
		var stageSpawn = HamsterWorld.GetOrCreateSystem(typeof(StageSpawnSystem));
		stageSpawn.Update(HamsterWorld.Unmanaged);
		
		var botSpawn = HamsterWorld.GetOrCreateSystem(typeof(BotSpawnSystem));
		botSpawn.Update(HamsterWorld.Unmanaged);
	}

	private void SetSystems()
	{
		var initializationSystemGroup = HamsterWorld.GetOrCreateSystemManaged<InitializationSystemGroup>();
		
		var presentation = HamsterWorld.GetOrCreateSystem(typeof(PresentationObjectSystem));
		initializationSystemGroup.AddSystemToUpdateList(presentation);
		
		var snakeSpawn = HamsterWorld.GetOrCreateSystem(typeof(SnakeSpawnSystem));
		initializationSystemGroup.AddSystemToUpdateList(snakeSpawn);
		
		var simulationSystemGroup = HamsterWorld.GetOrCreateSystemManaged<SimulationSystemGroup>();
		
		var inputSystem = HamsterWorld.GetOrCreateSystem(typeof(InputDetectionSystem));
		simulationSystemGroup.AddSystemToUpdateList(inputSystem);
		
		var population= HamsterWorld.GetOrCreateSystem(typeof(PopulationSystem));
		simulationSystemGroup.AddSystemToUpdateList(population);
		
		var botDecision = HamsterWorld.GetOrCreateSystem(typeof(BotDecisionSystem));
		simulationSystemGroup.AddSystemToUpdateList(botDecision);
		
		var snakeDecision = HamsterWorld.GetOrCreateSystem(typeof(SnakeDecisionSystem));
		simulationSystemGroup.AddSystemToUpdateList(snakeDecision);
		
		var orientationSystem = HamsterWorld.GetOrCreateSystem(typeof(OrientationSystem));
		simulationSystemGroup.AddSystemToUpdateList(orientationSystem);
		
		var snakeElementsSystem = HamsterWorld.GetOrCreateSystem(typeof(SnakeElementSystem));
		simulationSystemGroup.AddSystemToUpdateList(snakeElementsSystem);
		
		var destroy= HamsterWorld.GetOrCreateSystem(typeof(BotDestroySystem));
		simulationSystemGroup.AddSystemToUpdateList(destroy);
		
		var disable = HamsterWorld.GetOrCreateSystem(typeof(BotDisableSystem));
		simulationSystemGroup.AddSystemToUpdateList(disable);
		
		var transformSystemGroup = HamsterWorld.GetOrCreateSystemManaged<TransformSystemGroup>();
		
		var moveSystem = HamsterWorld.GetOrCreateSystem(typeof(MoveSystem));
		transformSystemGroup.AddSystemToUpdateList(moveSystem);
		
		var rotationSystem = HamsterWorld.GetOrCreateSystem(typeof(RotationSystem));
		transformSystemGroup.AddSystemToUpdateList(rotationSystem);
		
		var animationSystem = HamsterWorld.GetOrCreateSystem(typeof(AnimationSystem));
		transformSystemGroup.AddSystemToUpdateList(animationSystem);
		
		var lateSystemGroup = HamsterWorld.GetOrCreateSystemManaged<LateSimulationSystemGroup>();
		
		var endTurn= HamsterWorld.GetOrCreateSystem(typeof(TurnSystem));
		lateSystemGroup.AddSystemToUpdateList(endTurn);
		TurnSystem.ResetTimer();
		
		var gameOverSystem = HamsterWorld.GetOrCreateSystem(typeof(GameOverSystem));
		lateSystemGroup.AddSystemToUpdateList(gameOverSystem);
		
		var stageComplete= HamsterWorld.GetOrCreateSystem(typeof(CompleteStageSystem));
		lateSystemGroup.AddSystemToUpdateList(stageComplete);
		
		SystemState state = HamsterWorld.Unmanaged.ResolveSystemStateRef(stageComplete);
		CompleteStageSystem.ResetStage();
		state.Enabled = true;
	}

	private void ResetStage(int obj)
	{
		PopulationSystem.ResetPopulationCounter();
		
		RandomSeed = Random.Range(1, 101);
		TilesSpawnSystem.ResetTiles();
		
		HamsterWorld.EntityManager.CompleteAllTrackedJobs();
		DestroyTerrain();
		DestroyBots();
		DestroySnake();
		
		var playerReset = HamsterWorld.GetOrCreateSystem(typeof(PlayerResetSystem));
		playerReset.Update(HamsterWorld.Unmanaged);
		
		var terrainSpawn = HamsterWorld.GetOrCreateSystem(typeof(StageSpawnSystem));
		terrainSpawn.Update(HamsterWorld.Unmanaged);
		
		var botSpawn = HamsterWorld.GetOrCreateSystem(typeof(BotSpawnSystem));
		botSpawn.Update(HamsterWorld.Unmanaged);

		var complete = HamsterWorld.GetExistingSystem<CompleteStageSystem>();
		ref SystemState state = ref HamsterWorld.Unmanaged.ResolveSystemStateRef(complete);
		state.Enabled = true;
		
		TurnSystem.ResetTimer();
	}

	private void ResetGame()
	{
		PopulationSystem.SetStartingPopulation();
		SnakeSpawnSystem.Reset();
		GameOverSystem.Reset();
	}
	
	private void DestroyTerrain()
	{
		EntityUtils.DestroyEntitiesWithComponent<TerrainTag>(HamsterWorld);
	}

	private void DestroyBots()
	{
		EntityUtils.DestroyEntitiesWithComponent<BotComponent>(HamsterWorld);
	}

	private void DestroySnake()
	{
		SnakeDecisionSystem.ClearActions();
		EntityUtils.DestroyEntitiesWithComponent<SnakeTag>(HamsterWorld);
	}
}
