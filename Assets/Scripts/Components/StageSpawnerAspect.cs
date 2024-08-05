using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct StageSpawnerAspect :IAspect
{
    private readonly RefRW<StageSpawnerComponent> hamsterSpawner;
    public Entity PlayerEntity => hamsterSpawner.ValueRW.PlayerPrefab;
    
    public Entity WheelEntity => hamsterSpawner.ValueRW.WheelPrefab;
    public Entity Entity => hamsterSpawner.ValueRW.HamsterPrefab;
    
    public int2 PlayerPosition => hamsterSpawner.ValueRO.PlayerPosition;
    
    public Orientation PlayerOrientation => hamsterSpawner.ValueRO.PlayerOrientation;
    
    public int2 WheelPosition => hamsterSpawner.ValueRO.WheelPosition;
    
    public Orientation WhellOrientation => hamsterSpawner.ValueRO.WheelOrientation;

}
