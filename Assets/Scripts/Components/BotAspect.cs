using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public readonly partial struct BotAspect : IAspect
{
	private readonly RefRO<BotComponent> botComponent;
	private readonly RefRW<ActionComponent> actionComponent;
	private readonly RefRW<RandomComponent> randomComponent;
	private readonly RefRW<OrientationComponent> orientationComponent;
	private readonly RefRW<MoveComponent> moveComponent;
	private readonly RefRW<RotationComponent> rotationComponent;

	public float GetRandomValue(float min,float max)=> randomComponent.ValueRW.Value.NextFloat(min, max);

	public Actions GetAction => actionComponent.ValueRW.Action;
	public void SetAction(Actions action)
	{
		actionComponent.ValueRW.Action = action;
	}
	
	public OrientationComponent OrientationComponent => orientationComponent.ValueRW;
	
	public Orientation GetOrientation => orientationComponent.ValueRW.CurrentOrientation;
	
	public void SetNewOrientation(Orientation orientation,Tile tile)
	{
		orientationComponent.ValueRW = new OrientationComponent()
		{
			CurrentOrientation = orientation,
			CurrentTileCoordinates = tile.Coordinates
			
		};
	}
	
	public void SetOrientation(Orientation orientation)
	{
		orientationComponent.ValueRW.CurrentOrientation = orientation;
	}
	
	public void SetCoordinates(int2 coordinates)
	{
		orientationComponent.ValueRW.CurrentTileCoordinates = coordinates;
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
	
	
	public bool OnFinalTile => TilesSpawnSystem.isFinalTile(orientationComponent.ValueRO.CurrentTileCoordinates);
	public int2 Coordinates => orientationComponent.ValueRO.CurrentTileCoordinates;
	
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

	public bool CanMove(Tile tile)
	{
		if (tile == null || tile.Creature != Tile.CreatureType.None
		                 || tile.Type == Tile.TileType.Rocks)
			return false;
		return true;
	}

}