using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
		private void Execute (in BotComponent botComponent, ref ActionComponent actionComponent,
			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent,
			ref RotationComponent rotationComponent)
		{
			SetOrientation(ref actionComponent, ref orientationComponent, ref moveComponent, ref rotationComponent);
		}
	}
	
	public partial struct PlayerOrientationJob : IJobEntity
	{
		private void Execute (in PlayerComponent playerComponent, ref ActionComponent actionComponent,
			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent,
			ref RotationComponent rotationComponent)
		{
			SetOrientation(ref actionComponent, ref orientationComponent, ref moveComponent, ref rotationComponent,
				isPlayer:true);
		}
	}
	
	public partial struct SnakeHeadOrientationJob : IJobEntity
	{
		private void Execute (in SnakeHeadComponent snakeHeadComponent, ref ActionComponent actionComponent,
			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent
			,ref RotationComponent rotationComponent)
		{
			SetOrientation(ref actionComponent, ref orientationComponent, ref moveComponent, ref rotationComponent,
				isSnake:true);
		}
	}
	
	public partial struct SnakeBodyOrientationJob : IJobEntity
	{
		private void Execute (in SnakeBodyElementComponent snakeBodyElementComponent, ref ActionComponent actionComponent,
			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent,
			ref RotationComponent rotationComponent)
		{
			SetOrientation(ref actionComponent, ref orientationComponent, ref moveComponent, ref rotationComponent);
		}
	}
	
	private static void SetOrientation( ref ActionComponent actionComponent,
    			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent, 
			    ref RotationComponent rotationComponent,bool isPlayer = false, bool isSnake = false)
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
					    
					    if(isPlayer && !OrientationComponent.CanMove(newTile))
						    return;
					    
    					var oldTile = TilesSpawnSystem.GetTile(orientationComponent.CurrentTileCoordinates.x,
    						orientationComponent.CurrentTileCoordinates.y);
    					oldTile.Exit();
    
    					newTile.Enter();
					    
					    if(isSnake)
						   newTile.SnakeEnter();
					    
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