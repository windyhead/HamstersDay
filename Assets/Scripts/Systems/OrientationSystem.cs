using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(BotDecisionSystem))]
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
		new BotOrientationJob().Schedule();
		new PlayerOrientationJob().Schedule();
	}

	public partial struct PlayerOrientationJob : IJobEntity
	{
		private void Execute(ref LocalTransform transform, ref ActionComponent actionComponent,
			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent,in PlayerComponent playerComponent)
		{
			SetOrientation(ref transform, ref actionComponent, ref orientationComponent, ref moveComponent,isPlayer:true);
		}
	}
	
	public partial struct BotOrientationJob : IJobEntity
	{
		private void Execute(ref LocalTransform transform, ref ActionComponent actionComponent,
			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent,in BotComponent botComponent)
		{
			SetOrientation(ref transform, ref actionComponent, ref orientationComponent, ref moveComponent,isPlayer:false);
		}
	}
	
	private static void SetOrientation(ref LocalTransform transform, ref ActionComponent actionComponent,
    			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent,bool isPlayer)
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
					    
					    if(!OrientationComponent.CanMove(newTile,isPlayer))
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
    					transform.Rotation = newRotation;
    					orientationComponent.CurrentOrientation = newOrientation;
    					break;
    				}
    
    				case Actions.TurnRight:
    				{
    					actionComponent.Action = Actions.None;
    					var newOrientation = Orientation.Right;
    					if (orientationComponent.CurrentOrientation != Orientation.Up)
    						newOrientation = orientationComponent.CurrentOrientation - 1;
    					var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
    					transform.Rotation = newRotation;
    					orientationComponent.CurrentOrientation = newOrientation;
    					break;
    				}
    			} 
		    }
}