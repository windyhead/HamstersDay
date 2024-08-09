using System.Collections.Generic;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(BotDecisionSystem))]
partial struct SnakeDecisionSystem : ISystem
{
	public static List<Actions> SnakeActions = new List <Actions>();
	public void OnCreate(ref SystemState state)
	{
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
			if(bodyElement.Index == 0)
				return;
			
			var previous = bodyElement.Index - 1;
			if(SnakeActions.Count <= previous)
				return;
			var newAction = SnakeActions[previous];
			actionComponent.Action = newAction;
		}
	}

	public partial struct SnakeDecisionJob : IJobEntity
	{
		private void Execute(SnakeAspect aspect)
		{
			var random = aspect.GetRandomValue(0,10);
			if (random <= 1)
			{
				aspect.SetAction(Actions.None);
				return;
			}

			var canMoveForward = aspect.CanMoveForward;
			var canMoveLeft = aspect.CanMoveLeft;
			var canMoveRight = aspect.CanMoveRight;

			var newAction = Actions.None;
			
			if (canMoveForward)
			{
				if (random <= 8)
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
			else if(random <= 9)
				newAction = Actions.TurnLeft;
			else 
				newAction = Actions.TurnRight;
			
			aspect.SetAction(newAction);
			SnakeActions.Insert(0,newAction);
		}
	}
	
}
