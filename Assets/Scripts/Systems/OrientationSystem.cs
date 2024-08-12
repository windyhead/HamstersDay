using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

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

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		if (!GameController.PlayerInputReceived)
			return;
		new BotOrientationJob().Schedule();
		new PlayerOrientationJob().Schedule();
		new SnakeHeadOrientationJob().Schedule();
		new SnakeBodyOrientationJob().Schedule();
	}
	
	public partial struct BotOrientationJob : IJobEntity
	{
		private void Execute(BotAspect aspect)
		{
			var action = aspect.GetAction;
			if (action == Actions.None)
				return;
			
			aspect.SetAction(Actions.None);

			switch (action)
			{
				case Actions.Move:
				{
					var newTile = aspect.OrientationComponent.GetForwardTile();
					var oldTile = TilesSpawnSystem.GetTile(aspect.OrientationComponent.CurrentTileCoordinates.x,
						aspect.OrientationComponent.CurrentTileCoordinates.y);
					oldTile.Exit();

					newTile.Enter(Tile.CreatureType.Hamster);
					aspect.SetCoordinates(newTile.Coordinates);
					aspect.SetTargetPosition(newTile.Center);
					break;
				}
				case Actions.TurnLeft:
				{
					var newOrientation = Orientation.Up;
					if (aspect.GetOrientation != Orientation.Right)
						newOrientation = aspect.GetOrientation + 1;
					var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
					aspect.SetTargetRotation(newRotation);
					aspect.SetOrientation(newOrientation);
					break;
				}
    
				case Actions.TurnRight:
				{
					var newOrientation = Orientation.Right;
					if (aspect.GetOrientation != Orientation.Up)
						newOrientation = aspect.GetOrientation - 1;
					var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
					aspect.SetTargetRotation(newRotation);
					aspect.SetOrientation(newOrientation);
					break;
				}
			}
		}
	}

	public partial struct PlayerOrientationJob : IJobEntity
	{
		private void Execute(PlayerAspect aspect)
		{
			var action = aspect.GetAction;
			if (action == Actions.None)
				return;
			
			aspect.SetAction(Actions.None);

			switch (action)
			{
				case Actions.Move:
				{
					var newTile = aspect.OrientationComponent.GetForwardTile();
					if (!aspect.CanMove(newTile))
						return;

					var oldTile = TilesSpawnSystem.GetTile(aspect.OrientationComponent.CurrentTileCoordinates.x,
						aspect.OrientationComponent.CurrentTileCoordinates.y);
					oldTile.Exit();

					newTile.Enter(Tile.CreatureType.Hamster);
					aspect.SetCoordinates(newTile.Coordinates);
					aspect.SetTargetPosition(newTile.Center);
					break;
				}
				case Actions.TurnLeft:
				{
					var newOrientation = Orientation.Up;
					if (aspect.GetOrientation != Orientation.Right)
						newOrientation = aspect.GetOrientation + 1;
					var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
					aspect.SetTargetRotation(newRotation);
					aspect.SetOrientation(newOrientation);
					break;
				}
    
				case Actions.TurnRight:
				{
					var newOrientation = Orientation.Right;
					if (aspect.GetOrientation != Orientation.Up)
						newOrientation = aspect.GetOrientation - 1;
					var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
					aspect.SetTargetRotation(newRotation);
					aspect.SetOrientation(newOrientation);
					break;
				}
			}
		}
	}
	
	public partial struct SnakeHeadOrientationJob : IJobEntity
	{
		private void Execute (SnakeAspect aspect)
		{
			var action = aspect.GetAction;
			if (action == Actions.None)
				return;
			var orientationComponent = aspect.GetOrientation;
			var moveComponent = aspect.GetMoveComponent;
			var rotationComponent = aspect.GetRotationComponent;
			
			aspect.SetAction(Actions.None);

			switch (action)
			{
				case Actions.Move:
				{
					var newTile = orientationComponent.GetForwardTile();
					var oldTile = TilesSpawnSystem.GetTile(orientationComponent.CurrentTileCoordinates.x,
						orientationComponent.CurrentTileCoordinates.y);
					oldTile.Exit();

					newTile.Enter(Tile.CreatureType.Snake);

					orientationComponent.CurrentTileCoordinates = new int2(newTile.Coordinates);
					moveComponent.TargetPosition = newTile.Center;
					moveComponent.MoveFinished = false;
					break;
				}
				case Actions.TurnLeft:
				{
					var newOrientation = Orientation.Up;
					if (orientationComponent.CurrentOrientation != Orientation.Right)
						newOrientation = orientationComponent.CurrentOrientation + 1;
					var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
					rotationComponent.TargetRotation = newRotation;
					orientationComponent.CurrentOrientation = newOrientation;
					rotationComponent.RotationFinished = false;
					break;
				}
    
				case Actions.TurnRight:
				{
					var newOrientation = Orientation.Right;
					if (orientationComponent.CurrentOrientation != Orientation.Up)
						newOrientation = orientationComponent.CurrentOrientation - 1;
					var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
					rotationComponent.TargetRotation = newRotation;
					orientationComponent.CurrentOrientation = newOrientation;
					rotationComponent.RotationFinished = false;
					break;
				}
			}
		}
	}
	
	public partial struct SnakeBodyOrientationJob : IJobEntity
	{
		private void Execute (in SnakeBodyElementComponent snakeBodyElementComponent, ref ActionComponent actionComponent,
			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent,
			ref RotationComponent rotationComponent)
		{
			
			if (actionComponent.Action == Actions.None)
    				return;
    
    			switch (actionComponent.Action)
    			{
    				case Actions.Move:
    				{
    					actionComponent.Action = Actions.None;
    					var newTile = orientationComponent.GetForwardTile();
    
    					if (newTile == null)
    						return;
					    
    					var oldTile = TilesSpawnSystem.GetTile(orientationComponent.CurrentTileCoordinates.x,
    						orientationComponent.CurrentTileCoordinates.y);
    					oldTile.Exit();
    
    					newTile.Enter(Tile.CreatureType.Snake);
    					orientationComponent.CurrentTileCoordinates = new int2(newTile.Coordinates);
    					moveComponent.TargetPosition = newTile.Center;
    					moveComponent.MoveFinished = false;
    					break;
    				}
    				case Actions.TurnLeft:
    				{
    					actionComponent.Action = Actions.None;
    					var newOrientation = Orientation.Up;
    					if (orientationComponent.CurrentOrientation != Orientation.Right)
    						newOrientation = orientationComponent.CurrentOrientation + 1;
    					var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
					    rotationComponent.TargetRotation = newRotation;
    					orientationComponent.CurrentOrientation = newOrientation;
					    rotationComponent.RotationFinished = false;
    					break;
    				}
    
    				case Actions.TurnRight:
    				{
    					actionComponent.Action = Actions.None;
    					var newOrientation = Orientation.Right;
    					if (orientationComponent.CurrentOrientation != Orientation.Up)
    						newOrientation = orientationComponent.CurrentOrientation - 1;
    					var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
					    rotationComponent.TargetRotation = newRotation;
    					orientationComponent.CurrentOrientation = newOrientation;
					    rotationComponent.RotationFinished = false;
    					break;
    				}
    			} 
		}
	}
	
	private static void TurnRight(OrientationComponent orientationComponent,RotationComponent rotationComponent)
	{
		var newOrientation = Orientation.Right;
		if (orientationComponent.CurrentOrientation != Orientation.Up)
			newOrientation = orientationComponent.CurrentOrientation - 1;
		var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
		rotationComponent.TargetRotation = newRotation;
		orientationComponent.CurrentOrientation = newOrientation;
		rotationComponent.RotationFinished = false;
	}

	private static void TurnLeft(OrientationComponent orientationComponent,RotationComponent rotationComponent)
	{
		var newOrientation = Orientation.Up;
		if (orientationComponent.CurrentOrientation != Orientation.Right)
			newOrientation = orientationComponent.CurrentOrientation + 1;
		var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
		rotationComponent.TargetRotation = newRotation;
		orientationComponent.CurrentOrientation = newOrientation;
		rotationComponent.RotationFinished = false;
	}
}