using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct PlayerAspect : IAspect
{
	private readonly RefRO<PlayerComponent> playerComponent;
	private readonly RefRW<ActionComponent> actionComponent;
	private readonly RefRW<OrientationComponent> orientationComponent;
	private readonly RefRW<LocalTransform> transformComponent;
	
	public void SetAction(Actions action)
	{
		actionComponent.ValueRW.Action = action;
	}

	public void SetOrientation(Orientation orientation,Tile tile)
	{
		orientationComponent.ValueRW = new OrientationComponent()
		{
			CurrentOrientation = orientation,
			CurrentTileCoordinates = tile.Coordinates
			
		};
	}
	
	public void SetTransform(Tile tile)
	{
		transformComponent.ValueRW = new LocalTransform { Position = tile.Center, Scale = 3, Rotation = GetRotation() };
	}
	
	private Quaternion GetRotation()
	{
		return OrientationComponent.GetRotationByOrientation(orientationComponent.ValueRW.CurrentOrientation);
	}
	
}