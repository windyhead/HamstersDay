using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public readonly partial struct SnakeHeadAspect : IAspect, ICreature
{
	private readonly RefRO<SnakeHeadComponent> snakeHeadComponent;
	private readonly RefRW<ActionComponent> actionComponent;
	private readonly RefRW<RandomComponent> randomComponent;
	private readonly RefRW<OrientationComponent> orientationComponent;
	private readonly RefRW<MoveComponent> moveComponent;
	private readonly RefRW<RotationComponent> rotationComponent;

	public float GetRandomValue(float min,float max)=> randomComponent.ValueRW.Value.NextFloat(min, max);
	
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

	public void  SetTargetRotation(Quaternion target)
	{
		rotationComponent.ValueRW.TargetRotation = target;
		rotationComponent.ValueRW.RotationFinished = false;
	}

	public bool HasForwardTarget()
	{
		var tile = orientationComponent.ValueRW.GetForwardTile();
		return HasTarget(tile);
	}
	
	public bool HasLeftTarget()
	{
		var tile = orientationComponent.ValueRW.GetLeftTile();
		return HasTarget(tile);
	}

	public bool HasRightTarget()
	{
		var tile = orientationComponent.ValueRW.GetRightTile();
		return HasTarget(tile);
	}

	public bool CanMoveForward()
	{
		var tile = orientationComponent.ValueRW.GetForwardTile();
		return CanMove(tile);
	}
	
	public bool CanMoveLeft()
	{
		var tile = orientationComponent.ValueRW.GetLeftTile();
		return CanMove(tile);
	}
	
	public bool CanMoveRight()
	{
		var tile = orientationComponent.ValueRW.GetRightTile();
		return CanMove(tile);
	}
	
	private static bool HasTarget(Tile tile)
	{
		if (tile == null || tile.Creature != Tile.CreatureType.Hamster 
		                 || tile.Type != Tile.TileType.Plains || tile.IsFinal)
			return false;
		return true;
	}

	private static bool CanMove(Tile tile)
	{
		if (tile == null || tile.Creature == Tile.CreatureType.Snake 
		                 || tile.Type == Tile.TileType.Rocks || tile.IsFinal)
			return false;
		return true;
	}
}