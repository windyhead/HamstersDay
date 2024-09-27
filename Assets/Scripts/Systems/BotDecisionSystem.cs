using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PopulationSystem))]
partial struct BotDecisionSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
	}

	public void OnDestroy(ref SystemState state)
	{
	}
	
	public void OnUpdate(ref SystemState state)
	{
		if(!GameController.PlayerInputReceived)
			return;
		new BotDecisionJob().Schedule();
	}

	public partial struct BotDecisionJob : IJobEntity
	{
		private void Execute(BotAspect aspect)
		{
			if (!aspect.HasStamina)
			{
				aspect.SetAction(Actions.Rest);
				return;
			}
			
			var random = aspect.GetRandomValue(0,10);
			if (random <= 0.5f)
			{
				aspect.SetAction(Actions.Rest);
				return;
			}

			var canMoveForward = aspect.CanMoveForward();
			var canMoveLeft = aspect.CanMoveLeft();
			var canMoveRight = aspect.CanMoveRight();
			
			if (canMoveForward)
			{
				if (random <= 8)
					aspect.SetAction(Actions.Move);
			}
			else if(canMoveLeft && !canMoveRight)
			{
				aspect.SetAction(Actions.TurnLeft);
			}
			else if(canMoveRight && !canMoveLeft)
			{
				aspect.SetAction(Actions.TurnRight);
			}
			else if(random <= 9)
				aspect.SetAction(Actions.TurnLeft);
			else 
				aspect.SetAction(Actions.TurnRight);
		}
	}
}
