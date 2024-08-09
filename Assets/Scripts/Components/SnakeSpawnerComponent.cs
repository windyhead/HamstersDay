using Unity.Entities;
using Unity.Mathematics;

	public struct SnakeSpawnerComponent : IComponentData
	{
		public Entity SnakeHead;
		public Entity SnakeBodyElement;
		public Entity SnakeTail;
	}
