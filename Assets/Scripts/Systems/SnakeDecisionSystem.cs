using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(BotDecisionSystem))]
partial struct SnakeDecisionSystem : ISystem
{
	public static List<Actions> SnakeActions;
	public void OnCreate(ref SystemState state)
	{
		SnakeActions = new List <Actions>();
		GameController.OnSnakeDestroyed += ClearActions;
	}

	private void ClearActions()
	{
		SnakeActions.Clear();
	}

	public void OnDestroy(ref SystemState state)
	{
		GameController.OnSnakeDestroyed -= ClearActions;
	}

	public void OnUpdate(ref SystemState state)
	{
		if (!GameController.PlayerInputReceived)
			return;
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var buffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		var EM = state.EntityManager;
		new BodyElementsJob(){ECB = buffer,EM = EM}.Schedule();
		new SnakeDecisionJob().Schedule();
	}
	
	public partial struct BodyElementsJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public EntityManager EM;
		private void Execute(SnakeBodyElementComponent bodyElement,
			ref ActionComponent actionComponent,
			Entity entity)
		{
			var childs = EM.GetBuffer<LinkedEntityGroup>(entity);
			actionComponent.Action = Actions.None;
			if (SnakeActions.Count <= bodyElement.Index)
				return;
			var newAction = SnakeActions[bodyElement.Index];
			actionComponent.Action = newAction;

			if (newAction == Actions.TurnLeft || newAction == Actions.TurnRight)
				SetEntityHierarchyEnabled(false, ECB.AsParallelWriter(), 2, childs);
			else
				SetEntityHierarchyEnabled(true, ECB.AsParallelWriter(), 2, childs);
			//Debug.Log(newAction);
		}
	}

	public partial struct SnakeDecisionJob : IJobEntity
	{

		private void Execute(SnakeHeadAspect headAspect)
		{
			var hasForwardTarget = headAspect.HasForwardTarget();
			var hasLeftTarget = headAspect.HasLeftTarget();
			var hasRightTarget = headAspect.HasRightTarget();

			if (hasForwardTarget)
			{
				SetSnakeAction(headAspect, Actions.Move);
				return;
			}

			if (hasLeftTarget)
			{
				SetSnakeAction(headAspect, Actions.TurnLeft);
				return;
			}

			if (hasRightTarget)
			{
				SetSnakeAction(headAspect, Actions.TurnRight);
				return;
			}

			var newAction = Actions.None;
			var random = headAspect.GetRandomValue(0, 10);

			var canMoveForward = headAspect.CanMoveForward();
			var canMoveLeft = headAspect.CanMoveLeft();
			var canMoveRight = headAspect.CanMoveRight();

			if (canMoveForward && random <= 9)
			{
				newAction = Actions.Move;
			}
			else if (canMoveLeft && !canMoveRight)
			{
				newAction = Actions.TurnLeft;
			}
			else if (canMoveRight && !canMoveLeft)
			{
				newAction = Actions.TurnRight;
			}
			else if (random <= 9.5f)
				newAction = Actions.TurnLeft;
			else
				newAction = Actions.TurnRight;

			SetSnakeAction(headAspect, newAction);
		}

		private static void SetSnakeAction(SnakeHeadAspect headAspect, Actions newAction)
		{
			headAspect.SetAction(newAction);
			SnakeActions.Insert(0, newAction);
		}
	}
	public static void SetEntityHierarchyEnabled(bool enabled, EntityCommandBuffer.ParallelWriter commandBuffer, int chunkIndex,DynamicBuffer<LinkedEntityGroup> dinamic)
	{
		if (enabled)
		{
			commandBuffer.RemoveComponent<Disabled>(chunkIndex, dinamic[chunkIndex].Value);
		}
		else
		{
			commandBuffer.AddComponent<Disabled>(chunkIndex, dinamic[chunkIndex].Value);
		}
	}
}
