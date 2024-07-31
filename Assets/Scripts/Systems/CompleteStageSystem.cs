using System;
using Unity.Burst;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(EndTurnSystem))]

partial struct CompleteStageSystem : ISystem
{
	public static Action OnStageComplete;
	
	public void OnCreate(ref SystemState state)
	{
	}

	public void OnDestroy(ref SystemState state)
	{
		
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		if(!GameController.IsTurnFinished)
			return;
		new  CheckCompleteJob().Run();
	}
	
	public partial struct CheckCompleteJob : IJobEntity
	{
		private void Execute(ref OrientationComponent orientationComponent, ref PlayerComponent playerComponent)
		{
			if(TilesManager.isFinalTile(orientationComponent.CurrentTileCoordinates))
				OnStageComplete?.Invoke();
					
		}
	}
}
