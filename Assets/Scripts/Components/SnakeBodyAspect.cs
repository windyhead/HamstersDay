using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public readonly partial struct SnakeBodyAspect : IAspect, ICreature
{
	private readonly RefRO<SnakeBodyElementComponent> snakeBodyElementComponent;
	private readonly RefRW<ActionComponent> actionComponent;
	private readonly RefRW<OrientationComponent> orientationComponent;
	private readonly RefRW<MoveComponent> moveComponent;
	private readonly RefRW<RotationComponent> rotationComponent;
	
	public Actions GetAction()
	{
		return actionComponent.ValueRW.Action;
	}

	public void SetAction(Actions action)
	{
		actionComponent.ValueRW.Action = action;
	}

	public Orientation GetCurrentOrientation()
	{
		return orientationComponent.ValueRW.CurrentOrientation;
	}

	public Tile GetForwardTile()
	{
		return orientationComponent.ValueRW.GetForwardTile();
	}

	bool ICreature.CanMove(Tile tile)
	{
		return CanMove(tile);
	}

	public void SetOrientation(Orientation orientation)
	{
		orientationComponent.ValueRW.CurrentOrientation = orientation;
	}

	public void SetCoordinates(int2 coordinates)
	{
		orientationComponent.ValueRW.CurrentTileCoordinates = coordinates;
	}

	public int2 GetCoordinates()
	{
		return orientationComponent.ValueRW.CurrentTileCoordinates;
	}

	public void  SetTargetPosition(float3 target)
	{
		moveComponent.ValueRW.TargetPosition = target;
		moveComponent.ValueRW.MoveFinished = false;
	}

	public void SetTargetRotation(Quaternion target)
	{
		rotationComponent.ValueRW.TargetRotation = target;
		rotationComponent.ValueRW.RotationFinished = false;
	}
	
	private static bool CanMove(Tile tile)
	{
		if (tile == null || tile.Creature == Tile.CreatureType.Snake 
		                 || tile.Type == Tile.TileType.Rocks||tile.IsFinal)
			return false;
		return true;
	}
}