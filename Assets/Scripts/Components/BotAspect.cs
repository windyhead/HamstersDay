using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public readonly partial struct BotAspect : IAspect, ICreature
{
	private readonly RefRO<BotComponent> botComponent;
	private readonly RefRW<ActionComponent> actionComponent;
	private readonly RefRW<RandomComponent> randomComponent;
	private readonly RefRW<OrientationComponent> orientationComponent;
	private readonly RefRW<LocalTransform> transformComponent;
	private readonly RefRW<MoveComponent> moveComponent;
	private readonly RefRW<RotationComponent> rotationComponent;
	private readonly RefRW<StaminaComponent> staminaComponent;

	public float GetRandomValue(float min,float max)=> randomComponent.ValueRW.Value.NextFloat(min, max);

	public Random Random => randomComponent.ValueRW.Value;

	public Actions GetAction()
	{
		return actionComponent.ValueRW.CurrentAction;
	}
	
	public void SetAction(Actions action)
	{
		actionComponent.ValueRW.CurrentAction = action;
	}

	public Orientation GetCurrentOrientation()
	{
		return orientationComponent.ValueRW.CurrentOrientation;
	}
	
	public void SetNewOrientation(Orientation orientation,Tile tile)
	{
		orientationComponent.ValueRW = new OrientationComponent()
		{
			CurrentOrientation = orientation,
			CurrentTileCoordinates = tile.Coordinates
			
		};
	}

	public Tile GetForwardTile()
	{
		return orientationComponent.ValueRW.GetForwardTile();
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
	
	public void SetTransform(Tile tile)
	{
		transformComponent.ValueRW = new LocalTransform { Position = tile.Center, Scale = 3, Rotation = GetRotation() };
	}

	
	public bool OnFinalTile => TilesSpawnSystem.isFinalTile(orientationComponent.ValueRO.CurrentTileCoordinates);
	
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
	
	public bool HasStamina => staminaComponent.ValueRO.HasStamina();
	
	private Quaternion GetRotation()
	{
		return OrientationComponent.GetRotationByOrientation(orientationComponent.ValueRW.CurrentOrientation);
	}

}