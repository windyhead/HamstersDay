using Unity.Entities;
using Unity.Mathematics;

	public struct HamsterSpawnerComponent : IComponentData
	{
		public Entity PlayerPrefab;
		public Entity HamsterPrefab;
		public int HamstersCount;
		public int2 PlayerPosition;
		public Orientation PlayerOrientation;
	}
