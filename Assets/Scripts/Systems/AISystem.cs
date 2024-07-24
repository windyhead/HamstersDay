using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct AiSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		if(!InputSystem.PlayerInputReceived)
			return;
		new BotDecisionJob().Run();
	}

	public partial struct BotDecisionJob : IJobEntity
	{
		private void Execute(in BotComponent botComponent, ref ActionComponent actionComponent,
			ref RandomComponent randomComponent, in OrientationComponent orientation)
		{
			var random = randomComponent.Value.NextFloat(0, 10);
			if (random <= 1)
			{
				actionComponent.Action = Actions.None;
				return;
			}
			
			var canMoveForward = orientation.GetTileAvailable(Actions.Move);
			var canMoveLeft = orientation.GetTileAvailable(Actions.TurnLeft);
			var canMoveRight = orientation.GetTileAvailable(Actions.TurnRight);
			
			if (canMoveForward)
			{
				if (random <= 8)
					actionComponent.Action = Actions.Move;
			}
			else if(canMoveLeft && !canMoveRight)
			{
				actionComponent.Action = Actions.TurnLeft;
			}
			else if(canMoveRight && !canMoveLeft)
			{
				actionComponent.Action = Actions.TurnRight;
			}
			else if(random <= 9)
				actionComponent.Action = Actions.TurnLeft;
			else 
				actionComponent.Action = Actions.TurnRight;
		}
	}
}
