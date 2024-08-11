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
	}

	public void OnDestroy(ref SystemState state)
	{
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
		private void Execute(SnakeAspect aspect)
		{
			var hasForwardTarget = aspect.HasForwardTarget();
			var hasLeftTarget = aspect.HasLeftTarget();
			var hasRightTarget = aspect.HasRightTarget();
			
			if (hasForwardTarget)
			{
				SetSnakeAction(aspect, Actions.Move); 
				return;
			}
			if(hasLeftTarget)
			{
				SetSnakeAction(aspect, Actions.TurnLeft); 
				return;
			}
			if(hasRightTarget)
			{
				SetSnakeAction(aspect, Actions.TurnRight); 
				return;
			}
			
			var newAction = Actions.None;
			var random = aspect.GetRandomValue(0,10);
			
			var canMoveForward = aspect.CanMoveForward;
			var canMoveLeft = aspect.CanMoveLeft;
			var canMoveRight = aspect.CanMoveRight;
			
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
			
			SetSnakeAction(aspect, newAction);
		}

		private static void SetSnakeAction(SnakeAspect aspect, Actions newAction)
		{
			aspect.SetAction(newAction);
			SnakeActions.Insert(0,newAction);
		}
	}
}
