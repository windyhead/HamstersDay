using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct StageSpawnerAspect :IAspect
{
    private readonly RefRW<StageSpawnerComponent> hamsterSpawner;
    public Entity PlayerEntity => hamsterSpawner.ValueRW.PlayerPrefab;
    public Entity BotEntity => hamsterSpawner.ValueRW.HamsterPrefab;
    public Entity HouseEntity => hamsterSpawner.ValueRW.HousePrefab;
    
    public int2 PlayerPosition => hamsterSpawner.ValueRO.PlayerPosition;

    public Orientation PlayerOrientation => hamsterSpawner.ValueRO.PlayerOrientation;

    public int2 HousePosition => hamsterSpawner.ValueRO.HousePosition;

    public Orientation HouseOrientation => hamsterSpawner.ValueRO.HouseOrientation;

}
