using Unity.Entities;
using Unity.Mathematics;

public struct TilesSpawnerComponent : IComponentData
{
	public Entity TilePrefab;
	public int Width;
	public int Lenth;
	public float TileSize;
	public float3 Transform;
}
