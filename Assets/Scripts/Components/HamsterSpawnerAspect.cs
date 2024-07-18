using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public readonly partial struct HamsterSpawnerAspect :IAspect
{
    private readonly RefRW<HamsterSpawnerComponent> hamsterSpawner;
    public Entity Entity => hamsterSpawner.ValueRW.HamsterPrefab;
    public int Count => hamsterSpawner.ValueRO.HamstersCount;
}
