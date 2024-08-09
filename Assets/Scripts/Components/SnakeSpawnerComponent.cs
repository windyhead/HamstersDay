using Unity.Entities;
using Unity.Mathematics;

	public struct SnakeSpawnerComponent : IComponentData
	{
		public Entity SnakeHead;
		public Entity SnakeElement;
		public Entity SnakeTail;
	}
