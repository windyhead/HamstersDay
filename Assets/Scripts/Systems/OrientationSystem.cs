using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
		new OrientationJob().Schedule();
	}

	public partial struct OrientationJob : IJobEntity
	{
		private void Execute(ref LocalTransform transform, ref ActionComponent actionComponent,
			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent,ref RotationComponent rotationComponent)
		{
			SetOrientation(ref transform, ref actionComponent, ref orientationComponent, ref moveComponent, ref rotationComponent);
		}
	}
	
	private static void SetOrientation(ref LocalTransform transform, ref ActionComponent actionComponent,
    			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent, ref RotationComponent rotationComponent)
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
					    
					    if(!OrientationComponent.CanMove(newTile))
						    return;
					    
    					var oldTile = TilesSpawnSystem.GetTile(orientationComponent.CurrentTileCoordinates.x,
    						orientationComponent.CurrentTileCoordinates.y);
    					oldTile.Exit();
    
    					newTile.Enter();
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