using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class StageSpawnerAuthoring : MonoBehaviour
{
	public GameObject PlayerPrefab;
	public GameObject HamsterPrefab;
	public GameObject WheelPrefab;
	public int Count;
	public Orientation PlayerOrientation;
	public Orientation WheelOrientation;
}

class HamsterSpawnerBaker : Baker<StageSpawnerAuthoring>
{
	public override void Bake(StageSpawnerAuthoring authoring)
	{
		AddComponent(new StageSpawnerComponent
		{
			HamsterPrefab = GetEntity(authoring.HamsterPrefab),
			HamstersCount = authoring.Count,
			PlayerPrefab = GetEntity(authoring.PlayerPrefab),
			PlayerPosition = new int2(0,0),
			PlayerOrientation = authoring.PlayerOrientation,
			WheelPrefab = GetEntity(authoring.WheelPrefab),
			WheelPosition = TilesManager.GetFinalTileCoordinates,
			WheelOrientation = authoring.WheelOrientation
		});
	}
}
