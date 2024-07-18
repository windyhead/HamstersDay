using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class HamsterSpawnerAuthoring : MonoBehaviour
{
	public GameObject HamsterPrefab;
	public int Count;
}

class HamsterSpawnerBaker : Baker<HamsterSpawnerAuthoring>
{
	public override void Bake(HamsterSpawnerAuthoring authoring)
	{
		AddComponent(new HamsterSpawnerComponent
		{
			HamsterPrefab = GetEntity(authoring.HamsterPrefab),
			HamstersCount = authoring.Count
		});
	}
}
