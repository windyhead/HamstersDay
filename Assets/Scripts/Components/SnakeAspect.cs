using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct SnakeAspect : IAspect
{
	private readonly RefRO<SnakeHeadComponent> headComponent;
	private readonly RefRW<ActionComponent> actionComponent;
	private readonly RefRW<RandomComponent> randomComponent;
	private readonly RefRW<OrientationComponent> orientationComponent;
	private readonly RefRW<MoveComponent> moveComponent;
	private readonly RefRW<RotationComponent> rotationComponent;

	public float GetRandomValue(float min,float max)=> randomComponent.ValueRW.Value.NextFloat(min, max);

	public void SetAction(Actions action)
	{
		actionComponent.ValueRW.Action = action;
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

	public bool CanMoveForward => orientationComponent.ValueRW.GetTileAvailable(Actions.Move);
	public bool CanMoveLeft => orientationComponent.ValueRW.GetTileAvailable(Actions.TurnLeft);
	public bool CanMoveRight => orientationComponent.ValueRW.GetTileAvailable(Actions.TurnRight);
	
	public bool OnFinalTile => TilesSpawnSystem.isFinalTile(orientationComponent.ValueRO.CurrentTileCoordinates);
	
	public int2 Coordinates => orientationComponent.ValueRO.CurrentTileCoordinates;
	
	private static bool HasTarget(Tile forwardTile)
	{
		if (forwardTile == null || forwardTile.IsEmpty || forwardTile.Type != Tile.TileType.Plains)
			return false;
		return true;
	}
}