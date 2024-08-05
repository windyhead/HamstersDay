using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class StageSpawnerAuthoring : MonoBehaviour
{
	public GameObject PlayerPrefab;
	public GameObject HamsterPrefab;
	public GameObject WheelPrefab;
	public int Count;
	public int2 PlayerPosition;
	public Orientation PlayerOrientation;
	public int2 WheelPosition;
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
			PlayerPosition = authoring.PlayerPosition,
			PlayerOrientation = authoring.PlayerOrientation,
			WheelPrefab = GetEntity(authoring.WheelPrefab),
			WheelPosition = authoring.WheelPosition,
			WheelOrientation = authoring.WheelOrientation
		});
	}
}
