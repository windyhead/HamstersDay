using Unity.Entities;
using Unity.Mathematics;

	public struct StageSpawnerComponent : IComponentData
	{
		public Entity HamsterPrefab;
		public int HamstersCount;
		public Entity PlayerPrefab;
		public int2 PlayerPosition;
		public Orientation PlayerOrientation;
		public Entity WheelPrefab;
		public int2 WheelPosition;
		public Orientation WheelOrientation;
	}
