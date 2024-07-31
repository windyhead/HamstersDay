using System;
using Unity.Burst;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(TurnSystem))]

partial struct CompleteStageSystem : ISystem
{
	public static Action OnStageComplete;
	
	public void OnCreate(ref SystemState state)
	{
	}

	public void OnDestroy(ref SystemState state)
	{
		
	}
	
	public void OnUpdate(ref SystemState state)
	{
		if(!GameController.IsTurnFinished)
			return;
		new CheckCompleteJob().Schedule();
	}
	
	public partial struct CheckCompleteJob : IJobEntity
	{
		private void Execute(in OrientationComponent orientationComponent, in PlayerComponent playerComponent)
		{
			if(TilesManager.isFinalTile(orientationComponent.CurrentTileCoordinates))
				OnStageComplete?.Invoke();
					
		}
	}
}
