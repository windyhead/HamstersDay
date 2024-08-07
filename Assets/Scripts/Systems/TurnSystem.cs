using System;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(MoveSystem))]

partial struct TurnSystem : ISystem
{
	public static Action<int> OnTurnFinished;
	private static int CurrentTurn = 1;
	
	public void OnCreate(ref SystemState state)
	{
	}

	public void OnDestroy(ref SystemState state)
	{
	}
	
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
		
		foreach (var rComponent in SystemAPI.Query<RefRO<RotationComponent>>())
		{
			if(!rComponent.ValueRO.RotationFinished)
				return;
		}
		
		GameController.IsTurnFinished = true;
		CurrentTurn ++;
		OnTurnFinished?.Invoke(CurrentTurn);
	}

	public static void ResetTimer()
	{
		CurrentTurn = 1;
		OnTurnFinished?.Invoke(CurrentTurn);
	}
}
