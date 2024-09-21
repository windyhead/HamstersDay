using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct StageSpawnerAspect :IAspect
{
    private readonly RefRW<StageSpawnerComponent> stageSpawner;
    public Entity HamsterEntity => stageSpawner.ValueRW.HamsterPrefab;
    public Entity HouseEntity => stageSpawner.ValueRW.HousePrefab;
    
    public Entity GateEntity => stageSpawner.ValueRW.CelestialGate;
    
    public Entity StoneEntity => stageSpawner.ValueRW.StonePrefab;
    
    public Entity FlowerEntity => stageSpawner.ValueRW.FlowersPrefab;
    
    public Entity NutEntity => stageSpawner.ValueRW.NutPrefab;
    
    public int2 PlayerPosition => stageSpawner.ValueRO.PlayerPosition;

    public Orientation PlayerOrientation => stageSpawner.ValueRO.PlayerOrientation;

    public int2 HousePosition => stageSpawner.ValueRO.HousePosition;

    public Orientation HouseOrientation => stageSpawner.ValueRO.HouseOrientation;

    public int StoneCount => stageSpawner.ValueRO.StoneCount;
    
    public int FlowersCount => stageSpawner.ValueRO.FlowersCount;

}
