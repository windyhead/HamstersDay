using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[DisableAutoCreation]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(MoveSystem))]

partial struct EndTurnSystem : ISystem
{
	public static Action<int> OnTurnFinished;
	public static int CurrentTurn = 1;
	
	public void OnCreate(ref SystemState state)
	{
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		if (GameController.PlayerInputReceived)
			GameController.PlayerInputReceived = false;
		
		if(GameController.IsTurnFinished)
			return;
		foreach (var moveComponent in SystemAPI.Query<RefRO<MoveComponent>>())
		{
			if(!moveComponent.ValueRO.MoveFinished)
				return;
		}
		GameController.IsTurnFinished = true;
		CurrentTurn ++;
		OnTurnFinished?.Invoke(CurrentTurn);
	}
}
