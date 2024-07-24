using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HamsterSpawnerAuthoring : MonoBehaviour
{
	public GameObject PlayerPrefab;
	public GameObject HamsterPrefab;
	public int Count;
	public int2 PlayerPosition;
	public Orientation PlayerOrientation;
}

class HamsterSpawnerBaker : Baker<HamsterSpawnerAuthoring>
{
	public override void Bake(HamsterSpawnerAuthoring authoring)
	{
		AddComponent(new HamsterSpawnerComponent
		{
			PlayerPrefab = GetEntity(authoring.PlayerPrefab),
			HamsterPrefab = GetEntity(authoring.HamsterPrefab),
			HamstersCount = authoring.Count,
			PlayerPosition = authoring.PlayerPosition,
			PlayerOrientation = authoring.PlayerOrientation
		});
	}
}
