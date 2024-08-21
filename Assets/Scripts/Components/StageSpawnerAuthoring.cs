using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class StageSpawnerAuthoring : MonoBehaviour
{
	public GameObject HamsterPrefab;
	public GameObject HousePrefab;
	public GameObject GatePrefab;
	public GameObject StonePrefab;
	public GameObject FlowersPrefab;
	
	public int2 PlayerPosition;
	public Orientation PlayerOrientation;
	public int2 HousePosition;
	public Orientation HouseOrientation;

	public int StoneCount;
	public int FlowersCount;
}

class StageSpawnerBaker : Baker<StageSpawnerAuthoring>
{
	public override void Bake(StageSpawnerAuthoring authoring)
	{
		AddComponent(new StageSpawnerComponent
		{
			HamsterPrefab = GetEntity(authoring.HamsterPrefab),
			HousePrefab = GetEntity(authoring.HousePrefab),
			CelestialGate = GetEntity(authoring.GatePrefab),
			StonePrefab = GetEntity(authoring.StonePrefab),
			FlowersPrefab = GetEntity(authoring.FlowersPrefab),
			PlayerPosition = authoring.PlayerPosition,
			PlayerOrientation = authoring.PlayerOrientation,
			HousePosition = authoring.HousePosition,
			HouseOrientation = authoring.HouseOrientation,
			StoneCount = authoring.StoneCount,
			FlowersCount = authoring.FlowersCount
		});
	}
}
