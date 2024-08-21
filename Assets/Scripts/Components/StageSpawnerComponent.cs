using Unity.Entities;
using Unity.Mathematics;

	public struct StageSpawnerComponent : IComponentData
	{
		public Entity HamsterPrefab;
		public Entity HousePrefab;
		public Entity CelestialGate;
		public Entity StonePrefab;
		public Entity FlowersPrefab;
		public int2 PlayerPosition;
		public Orientation PlayerOrientation;
		public int2 HousePosition;
		public Orientation HouseOrientation;
		public int StoneCount;
		public int FlowersCount;
	}
