using Unity.Entities;
using Unity.Transforms;
using Random = UnityEngine.Random;

public class SystemsController : SingletonBehaviour<SystemsController>
{
	public static int RandomSeed { get; private set; }
	
	private static World HamsterWorld;
	
	public static void CreateWorld()
	{
		HamsterWorld = World.DefaultGameObjectInjectionWorld;
	}
	
	public static void SetStartingPopulation()
	{
		PopulationSystem.SetStartingPopulation();
	}
	
	public static void SetStage()
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

	public static void SetSystems()
	{
		var initializationSystemGroup = HamsterWorld.GetOrCreateSystemManaged<InitializationSystemGroup>();
		
		var presentation = HamsterWorld.GetOrCreateSystem(typeof(PresentationObjectSystem));
		initializationSystemGroup.AddSystemToUpdateList(presentation);
		
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
		
		var snakeElementsSystem = HamsterWorld.GetOrCreateSystem(typeof(SnakeElementSystem));
		simulationSystemGroup.AddSystemToUpdateList(snakeElementsSystem);
		
		var destroy= HamsterWorld.GetOrCreateSystem(typeof(BotDestroySystem));
		simulationSystemGroup.AddSystemToUpdateList(destroy);
		
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
		
		var population= HamsterWorld.GetOrCreateSystem(typeof(PopulationSystem));
		lateSystemGroup.AddSystemToUpdateList(population);
		
		var stageComplete= HamsterWorld.GetOrCreateSystem(typeof(CompleteStageSystem));
		lateSystemGroup.AddSystemToUpdateList(stageComplete);
		
		SystemState state = HamsterWorld.Unmanaged.ResolveSystemStateRef(stageComplete);
		CompleteStageSystem.ResetStage();
		state.Enabled = true;
	}
	
	public static void ResetStage()
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

	public static void ResetGame()
	{
		PopulationSystem.SetStartingPopulation();
		SnakeSpawnSystem.Reset();
		GameOverSystem.Reset();
	}
	
	private static void DestroyTerrain()
	{
		EntityUtils.DestroyEntitiesWithComponent<TerrainTag>(HamsterWorld);
	}

	private static void DestroyBots()
	{
		EntityUtils.DestroyEntitiesWithComponent<BotComponent>(HamsterWorld);
	}

	private static void DestroySnake()
	{
		SnakeDecisionSystem.ClearActions();
		EntityUtils.DestroyEntitiesWithComponent<SnakeTag>(HamsterWorld);
	}
}
