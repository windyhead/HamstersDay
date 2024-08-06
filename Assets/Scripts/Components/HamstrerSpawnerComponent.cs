using Unity.Entities;
using Unity.Mathematics;

	public struct StageSpawnerComponent : IComponentData
	{
		public Entity HamsterPrefab;
		public Entity PlayerPrefab;
		public Entity HousePrefab;
		public int2 PlayerPosition;
		public Orientation PlayerOrientation;
		public int2 HousePosition;
		public Orientation HouseOrientation;
	}
