using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class GameController : SingletonBehaviour
{
	public static World HamsterWorld { get; private set; }
	public static bool IsGameStarted { get; private set; }

	public static bool PlayerInputReceived;
	
	public static bool IsTurnFinished = true;

	public static int RandomNumber { get; private set; }

	private void Awake()
	{
		HamsterWorld = World.DefaultGameObjectInjectionWorld;
		CompleteStageSystem.OnStageComplete += NextStage;
	}
	
	private void Start()
	{
		StartCoroutine(StartGame());
	}
	
	private IEnumerator StartGame()
	{
		RandomNumber = Random.Range(1, 101);
		yield return new WaitForEndOfFrame();
		var tileSpawn = HamsterWorld.GetOrCreateSystem(typeof(TilesSpawnSystem));
		tileSpawn.Update(HamsterWorld.Unmanaged);
		var playerSpawn = HamsterWorld.GetOrCreateSystem(typeof(PlayerSpawnSystem));
		playerSpawn.Update(HamsterWorld.Unmanaged);
		var wheelSpawn = HamsterWorld.GetOrCreateSystem(typeof(WheelSpawnSystem));
		wheelSpawn.Update(HamsterWorld.Unmanaged);
		var botSpawn = HamsterWorld.GetOrCreateSystem(typeof(BotSpawnSystem));
		botSpawn.Update(HamsterWorld.Unmanaged);
		 
		var simulationSystemGroup = HamsterWorld.GetOrCreateSystemManaged<SimulationSystemGroup>();
		
		var inputSystem = HamsterWorld.GetOrCreateSystem(typeof(InputDetectionSystem));
		simulationSystemGroup.AddSystemToUpdateList(inputSystem);
		
		var aiSystem = HamsterWorld.GetOrCreateSystem(typeof(AISystem));
		simulationSystemGroup.AddSystemToUpdateList(aiSystem);
		
		var orientationSystem = HamsterWorld.GetOrCreateSystem(typeof(OrientationSystem));
		simulationSystemGroup.AddSystemToUpdateList(orientationSystem);
		
		var transformSystemGroup = HamsterWorld.GetOrCreateSystemManaged<TransformSystemGroup>();
		
		var moveSystem = HamsterWorld.GetOrCreateSystem(typeof(MoveSystem));
		transformSystemGroup.AddSystemToUpdateList(moveSystem);
		
		var lateSystemGroup = HamsterWorld.GetOrCreateSystemManaged<LateSimulationSystemGroup>();
		
		var endTurn= HamsterWorld.GetOrCreateSystem(typeof(TurnSystem));
		lateSystemGroup.AddSystemToUpdateList(endTurn);
		
		var stageComplete= HamsterWorld.GetOrCreateSystem(typeof(CompleteStageSystem));
		lateSystemGroup.AddSystemToUpdateList(stageComplete);
		IsGameStarted = true;
	}

	public void OnDestroy()
	{
		World.DisposeAllWorlds();
	}

	private void NextStage()
	{
		TurnSystem.ResetTimer();
	}
}
