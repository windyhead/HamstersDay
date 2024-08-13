using Unity.Burst;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SnakeDecisionSystem))]
partial struct OrientationSystem : ISystem
{
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
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var buffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		new BotOrientationJob().Schedule();
		new PlayerOrientationJob().Schedule();
		new SnakeHeadOrientationJob().Schedule();
		new SnakeBodyOrientationJob{}.Schedule();
	}
	
	public partial struct BotOrientationJob : IJobEntity
	{
		private void Execute(BotAspect aspect)
		{
			var action = aspect.GetAction();
			if (action == Actions.None)
				return;
			
			aspect.SetAction(Actions.None);

			switch (action)
			{
				case Actions.Move:
				{
					var newTile = aspect.GetForwardTile();
					var oldTile = TilesSpawnSystem.GetTile(aspect.GetCoordinates().x, 
						aspect.GetCoordinates().y);
					oldTile.Exit();
					newTile.Enter(Tile.CreatureType.Hamster);
					aspect.SetCoordinates(newTile.Coordinates);
					aspect.SetTargetPosition(newTile.Center);
					break;
				}
				case Actions.TurnLeft:
				{
					TurnLeft(aspect);
					break;
				}
    
				case Actions.TurnRight:
				{
					TurnRight(aspect);
					break;
				}
			}
		}
	}

	public partial struct PlayerOrientationJob : IJobEntity
	{
		private void Execute(PlayerAspect aspect)
		{
			var action = aspect.GetAction();
			if (action == Actions.None)
				return;
			
			aspect.SetAction(Actions.None);

			switch (action)
			{
				case Actions.Move:
				{
					var newTile = aspect.GetForwardTile();
					if (!aspect.CanMove(newTile))
						return;

					var oldTile = TilesSpawnSystem.GetTile(aspect.GetCoordinates().x,
						aspect.GetCoordinates().y);
					oldTile.Exit();

					newTile.Enter(Tile.CreatureType.Hamster);
					aspect.SetCoordinates(newTile.Coordinates);
					aspect.SetTargetPosition(newTile.Center);
					break;
				}
				case Actions.TurnLeft:
				{
					TurnLeft(aspect);
					break;
				}
    
				case Actions.TurnRight:
				{
					TurnRight(aspect);
					break;
				}
			}
		}
	}
	
	public partial struct SnakeHeadOrientationJob : IJobEntity
	{
		private void Execute(SnakeHeadAspect headAspect)
		{
			var action = headAspect.GetAction();
			if (action == Actions.None)
				return;
			
			headAspect.SetAction(Actions.None);

			switch (action)
			{
				case Actions.Move:
				{
					var newTile = headAspect.GetForwardTile();
					var oldTile = TilesSpawnSystem.GetTile(headAspect.GetCoordinates().x, 
						headAspect.GetCoordinates().y);
					oldTile.Exit();
					newTile.Enter(Tile.CreatureType.Snake);
					headAspect.SetCoordinates(newTile.Coordinates);
					headAspect.SetTargetPosition(newTile.Center);
					break;
				}
				case Actions.TurnLeft:
				{
					TurnLeft(headAspect);
					break;
				}
    
				case Actions.TurnRight:
				{
					TurnRight(headAspect);
					break;
				}
			}
		}
	}
	
	public partial struct SnakeBodyOrientationJob : IJobEntity
	{
		
		private void Execute(SnakeBodyAspect aspect)
		{
			var action = aspect.GetAction();
			if (action == Actions.None)
				return;
			
			aspect.SetAction(Actions.None);

			switch (action)
			{
				case Actions.Move:
				{
					var newTile = aspect.GetForwardTile();
					var oldTile = TilesSpawnSystem.GetTile(aspect.GetCoordinates().x, 
						aspect.GetCoordinates().y);
					oldTile.Exit();
					newTile.Enter(Tile.CreatureType.Snake);
					aspect.SetCoordinates(newTile.Coordinates);
					aspect.SetTargetPosition(newTile.Center);
					break;
				}
				case Actions.TurnLeft:
				{
					TurnLeft(aspect);
					break;
				}
    
				case Actions.TurnRight:
				{
					TurnRight(aspect);
					break;
				}
			}
		}
	}
	
	private static void TurnRight(ICreature aspect)
	{
		var newOrientation = Orientation.Right;
		if (aspect.GetCurrentOrientation() != Orientation.Up)
			newOrientation = aspect.GetCurrentOrientation() - 1;
		var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
		aspect.SetTargetRotation(newRotation);
		aspect.SetOrientation(newOrientation);
	}

	private static void TurnLeft(ICreature aspect)
	{
		var newOrientation = Orientation.Up;
		if (aspect.GetCurrentOrientation() != Orientation.Right)
			newOrientation = aspect.GetCurrentOrientation() + 1;
		var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
		aspect.SetTargetRotation(newRotation);
		aspect.SetOrientation(newOrientation);
	}
}