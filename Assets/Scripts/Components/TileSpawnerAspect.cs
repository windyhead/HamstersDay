using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct TileSpawnerAspect : IAspect
{
	private readonly RefRO<TilesSpawnerComponent> tilesSpawner;

	public int Width => tilesSpawner.ValueRO.Width;
	public int Length => tilesSpawner.ValueRO.Lenth;
	public float TileSize => tilesSpawner.ValueRO.TileSize;
	
	public float OffsetX => (Width - 1) * TileSize * 0.5f;
	
	public float OffsetZ => (Length - 1) * TileSize * 0.5f;

	public float3 Transform => tilesSpawner.ValueRO.Transform;
	public Entity Entity => tilesSpawner.ValueRO.TilePrefab;
}