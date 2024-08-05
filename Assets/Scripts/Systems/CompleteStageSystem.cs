using System;
using Unity.Burst;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(TurnSystem))]

partial struct CompleteStageSystem : ISystem
{
	public static Action OnStageComplete;
	private static bool stageComplete;
	
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<PlayerComponent>();
	}

	public void OnDestroy(ref SystemState state)
	{
		
	}
	
	public void OnUpdate(ref SystemState state)
	{
		if (stageComplete)
		{
			state.Enabled = false;
			ResetStage();
			OnStageComplete?.Invoke();
		}
		
		if(!GameController.IsTurnFinished)
			return;
		
		new CheckCompleteJob().Schedule();
	}
	
	public partial struct CheckCompleteJob : IJobEntity
	{
		private void Execute(in OrientationComponent orientationComponent, in PlayerComponent playerComponent)
		{
			if (TilesSpawnSystem.isFinalTile(orientationComponent.CurrentTileCoordinates))
				stageComplete = true;
		}
	}

	public static void ResetStage()
	{
		stageComplete = false;
	}
}
