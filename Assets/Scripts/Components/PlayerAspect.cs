using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct PlayerAspect : IAspect, ICreature
{
	private readonly RefRO<PlayerComponent> playerComponent;
	private readonly RefRW<ActionComponent> actionComponent;
	private readonly RefRW<OrientationComponent> orientationComponent;
	private readonly RefRW<LocalTransform> transformComponent;
	private readonly RefRW<MoveComponent> moveComponent;
	private readonly RefRW<RotationComponent> rotationComponent;
	private readonly RefRW<HamsterComponent> hamsterComponent;
	private readonly RefRW<StaminaComponent> staminaComponent;

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

	public Tile GetForwardTile()
	{
		return orientationComponent.ValueRW.GetForwardTile();
	}

	public OrientationComponent OrientationComponent => orientationComponent.ValueRW;
	
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
	
	public Orientation GetOrientation => orientationComponent.ValueRW.CurrentOrientation;
	
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
	
	public bool CanMove(Tile tile)
	{
		if (tile == null || tile.Creature != Tile.CreatureType.None 
		                 || tile.Type == Tile.TileType.Rocks)
			return false;
		return true;
	}

	public void ResetStamina()
	{
		staminaComponent.ValueRW.Reset();
	}
	
	public void ResetFat()
	{
		hamsterComponent.ValueRW.Reset();
	}

	private Quaternion GetRotation()
	{
		return OrientationComponent.GetRotationByOrientation(orientationComponent.ValueRW.CurrentOrientation);
	}
}