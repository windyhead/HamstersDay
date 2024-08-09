using Unity.Entities;
using UnityEngine;

public class SnakeSpawnerAuthoring : MonoBehaviour
{
	public GameObject SnakeHeadPrefab;
	public GameObject SnakeElementPrefab;
	public GameObject SnakeTailPrefab;
}

class SnakeSpawnerBaker : Baker<SnakeSpawnerAuthoring>
{
	public override void Bake(SnakeSpawnerAuthoring authoring)
	{
		AddComponent(new SnakeSpawnerComponent
		{
			SnakeHead = GetEntity(authoring.SnakeHeadPrefab),
			SnakeElement = GetEntity(authoring.SnakeElementPrefab),
			SnakeTail = GetEntity(authoring.SnakeElementPrefab)
		});
	}
}
