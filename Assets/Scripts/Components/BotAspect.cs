using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct BotAspect : IAspect
{
	private readonly RefRO<BotComponent> botComponent;
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
	public bool CanMoveForward => orientationComponent.ValueRW.GetTileAvailable(Actions.Move);
	public bool CanMoveLeft => orientationComponent.ValueRW.GetTileAvailable(Actions.TurnLeft);
	public bool CanMoveRight => orientationComponent.ValueRW.GetTileAvailable(Actions.TurnRight);
	public bool OnFinalTile => TilesSpawnSystem.isFinalTile(orientationComponent.ValueRO.CurrentTileCoordinates);
	public int2 Coordinates => orientationComponent.ValueRO.CurrentTileCoordinates;

}