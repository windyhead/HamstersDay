using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class StageSpawnerAuthoring : MonoBehaviour
{
	public GameObject PlayerPrefab;
	public GameObject HamsterPrefab;
	public GameObject HousePrefab;
	public GameObject StonePrefab;
	
	public int2 PlayerPosition;
	public Orientation PlayerOrientation;
	public int2 HousePosition;
	public Orientation HouseOrientation;

	public int StoneCount;
}

class StageSpawnerBaker : Baker<StageSpawnerAuthoring>
{
	public override void Bake(StageSpawnerAuthoring authoring)
	{
		AddComponent(new StageSpawnerComponent
		{
			PlayerPrefab = GetEntity(authoring.PlayerPrefab),
			HamsterPrefab = GetEntity(authoring.HamsterPrefab),
			HousePrefab = GetEntity(authoring.HousePrefab),
			StonePrefab = GetEntity(authoring.StonePrefab),
			PlayerPosition = authoring.PlayerPosition,
			PlayerOrientation = authoring.PlayerOrientation,
			HousePosition = authoring.HousePosition,
			HouseOrientation = authoring.HouseOrientation,
			StoneCount = authoring.StoneCount
		});
	}
}
