using Unity.Entities;

public readonly partial struct SnakeAspect : IAspect
{
	private readonly RefRO<SnakeHeadComponent> headComponent;
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
	
	public OrientationComponent GetOrientation => orientationComponent.ValueRW;
	
	public MoveComponent GetMoveComponent => moveComponent.ValueRW;
	
	public RotationComponent GetRotationComponent => rotationComponent.ValueRW;

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
		                 || tile.Type == Tile.TileType.Rocks||tile.IsFinal)
			return false;
		return true;
	}
}