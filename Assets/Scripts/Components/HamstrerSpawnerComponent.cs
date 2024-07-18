using Unity.Entities;

public struct HamsterSpawnerComponent : IComponentData
{
	public Entity HamsterPrefab;
	public int HamstersCount;
	public uint RandomSeed;
}
