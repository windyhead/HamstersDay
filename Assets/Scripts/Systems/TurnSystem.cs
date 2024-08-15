using System;
using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(MoveSystem))]

partial struct TurnSystem : ISystem
{
	public static Action<int> OnTurnFinished;

	public static bool IsTurnFinished { get; private set; }
	public static int CurrentTurn { get; private set; } = 1;

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
		//Debug.Log("MoveFinished");
		
		foreach (var rComponent in SystemAPI.Query<RefRO<RotationComponent>>())
		{
			if(!rComponent.ValueRO.RotationFinished)
				return;
		}
		//Debug.Log("RotationFinished");
		
		GameController.IsTurnFinished = true;
		CurrentTurn ++;
		//Debug.Log("Turn_" + CurrentTurn);
		OnTurnFinished?.Invoke(CurrentTurn);
	}

	public static void ResetTimer()
	{
		CurrentTurn = 1;
		OnTurnFinished?.Invoke(CurrentTurn);
	}

}
