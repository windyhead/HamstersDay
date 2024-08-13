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
		
		new BodyElementsJob().Schedule();
		new SnakeDecisionJob().Schedule();
	}
	
	public partial struct BodyElementsJob : IJobEntity
	{
		private void Execute(SnakeBodyElementComponent bodyElement,ref ActionComponent actionComponent)
		{
			actionComponent.Action = Actions.None;
			if (SnakeActions.Count <= bodyElement.Index)
				return;
			var newAction = SnakeActions[bodyElement.Index];
			actionComponent.Action = newAction;
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
			if(hasLeftTarget)
			{
				SetSnakeAction(headAspect, Actions.TurnLeft); 
				return;
			}
			if(hasRightTarget)
			{
				SetSnakeAction(headAspect, Actions.TurnRight); 
				return;
			}
			
			var newAction = Actions.None;
			var random = headAspect.GetRandomValue(0,10);
			
			var canMoveForward = headAspect.CanMoveForward();
			var canMoveLeft = headAspect.CanMoveLeft();
			var canMoveRight = headAspect.CanMoveRight();
			
			if (canMoveForward && random <= 9)
			{
				newAction = Actions.Move;
			}
			else if(canMoveLeft && !canMoveRight)
			{
				newAction = Actions.TurnLeft;
			}
			else if(canMoveRight && !canMoveLeft)
			{
				newAction = Actions.TurnRight;
			}
			else if(random <= 9.5f)
				newAction = Actions.TurnLeft;
			else 
				newAction = Actions.TurnRight;
			
			SetSnakeAction(headAspect, newAction);
		}

		private static void SetSnakeAction(SnakeHeadAspect headAspect, Actions newAction)
		{
			headAspect.SetAction(newAction);
			SnakeActions.Insert(0,newAction);
		}
	}
}
