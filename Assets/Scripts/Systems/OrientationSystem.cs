using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(TransformSystemGroup))]
partial struct OrientationSystem : ISystem
{
	public void OnCreate(ref SystemState state) { }

	public void OnDestroy(ref SystemState state) { }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		new OrientationJob().Schedule();
	}

	public partial struct OrientationJob : IJobEntity
	{
		private void Execute(ref LocalTransform  transform,ref ActionComponent actionComponent,
			ref OrientationComponent orientationComponent, ref MoveComponent moveComponent)
		{
			if(actionComponent.Action == Actions.None)
				return;
			
			switch (actionComponent.Action)
			{
				case Actions.Move:
				{
					actionComponent.Action = Actions.None;
					var newTile = orientationComponent.GetForwardTile();
					
					if(newTile == null)
						return;
					
					if(!newTile.isEmpty)
						return;
					
					var oldTile = TilesManager.GetTile(orientationComponent.CurrentTileCoordinates.x,
						orientationComponent.CurrentTileCoordinates.y);
					oldTile.isEmpty = true;
				
					newTile.isEmpty = false;
					orientationComponent.CurrentTileCoordinates = new int2(newTile.RowNumber, newTile.ColumnNumber);
					moveComponent.TargetPosition = newTile.Center;
					moveComponent.MoveFinished = false;
					break;
				}
				case Actions.TurnLeft:
				{
					actionComponent.Action = Actions.None;
					var newOrientation = Orientation.Up;
					if(orientationComponent.CurrentOrientation != Orientation.Right)
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
					if(orientationComponent.CurrentOrientation != Orientation.Up)
						newOrientation = orientationComponent.CurrentOrientation - 1;
					var newRotation =  OrientationComponent.GetRotationByOrientation(newOrientation);
					transform.Rotation = newRotation;
					orientationComponent.CurrentOrientation = newOrientation;
					break;
				}
			}
		}
	}
}
