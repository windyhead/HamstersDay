using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct StageSpawnerAspect :IAspect
{
    private readonly RefRW<StageSpawnerComponent> stageSpawner;
    public Entity PlayerEntity => stageSpawner.ValueRW.PlayerPrefab;
    public Entity BotEntity => stageSpawner.ValueRW.HamsterPrefab;
    public Entity HouseEntity => stageSpawner.ValueRW.HousePrefab;
    
    public Entity StoneEntity => stageSpawner.ValueRW.StonePrefab;
    
    public int2 PlayerPosition => stageSpawner.ValueRO.PlayerPosition;

    public Orientation PlayerOrientation => stageSpawner.ValueRO.PlayerOrientation;

    public int2 HousePosition => stageSpawner.ValueRO.HousePosition;

    public Orientation HouseOrientation => stageSpawner.ValueRO.HouseOrientation;

    public int StoneCount => stageSpawner.ValueRO.StoneCount;

}
