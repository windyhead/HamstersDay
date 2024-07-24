using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct HamsterSpawnerAspect :IAspect
{
    private readonly RefRW<HamsterSpawnerComponent> hamsterSpawner;
    public Entity PlayerEntity => hamsterSpawner.ValueRW.PlayerPrefab;
    public Entity Entity => hamsterSpawner.ValueRW.HamsterPrefab;
    public int Count => hamsterSpawner.ValueRO.HamstersCount;

    public int2 PlayerPosition => hamsterSpawner.ValueRO.PlayerPosition;
    public Orientation PlayerOrientation => hamsterSpawner.ValueRO.PlayerOrientation;

}
