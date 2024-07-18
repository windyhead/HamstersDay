using Unity.Entities;
using UnityEngine;
public class TilesSpawnerAuthoring : MonoBehaviour
{
	public GameObject TilePrefab;
	public Transform Transform;
	public int Width;
	public int Lenth;
	public float Size;
}

class TilesSpawnerBaker : Baker<TilesSpawnerAuthoring>
{
	public override void Bake(TilesSpawnerAuthoring authoring)
	{
		AddComponent(new TilesSpawnerComponent
		{
			TilePrefab = GetEntity(authoring.TilePrefab),
			Width = authoring.Width,
			Lenth = authoring.Lenth,
			Transform = authoring.Transform.position,
			TileSize = authoring.Size
		});
	}
}
