using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = false)]
[UpdateAfter(typeof(BotSpawnSystem))]
partial struct SnakeSpawnSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<SnakeSpawnerComponent>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	public void OnUpdate(ref SystemState state)
	{
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var buffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		new SnakeSpawnSystemJob() { ECB = buffer, RandomNumber = GameController.RandomSeed }.Schedule();
	}

	public partial struct SnakeSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public int RandomNumber;

		private void Execute(SnakeSpawnerAspect aspect)
		{
			var snakeHead = ECB.Instantiate(aspect.HeadEntity);
			ECB.SetName(snakeHead,"SNAKEHEAD");
			
			var headComponent = new SnakeHeadComponent();
			ECB.AddComponent(snakeHead, headComponent);
			
			var random = Random.CreateFromIndex((uint)(RandomNumber));
			var randomComponent = new RandomComponent() { Value = random };
			ECB.AddComponent(snakeHead, randomComponent);
			
			var actionComponent = new ActionComponent() { Action = Actions.None };
			ECB.AddComponent<ActionComponent>(snakeHead, actionComponent);
			
			var randomOrientationNumber = random.NextInt(0, Enum.GetValues(typeof(Orientation)).Length);
			var orientation = (Orientation)randomOrientationNumber;
			var tile = TilesSpawnSystem.GetRandomTileOnBorders(random, orientation);
			tile.Enter();
			
			var orientationComponent = new OrientationComponent()
				{ CurrentOrientation = orientation, CurrentTileCoordinates = tile.Coordinates };
			ECB.AddComponent(snakeHead, orientationComponent);
			
			var rotation = OrientationComponent.GetRotationByOrientation(orientation);
			ECB.AddComponent(snakeHead, new RotationComponent() { RotationFinished = true });
			
			ECB.AddComponent(snakeHead, new MoveComponent { MoveFinished = true });
			
			ECB.SetComponent(snakeHead,
				new LocalTransform { Position = tile.Center, Scale = 3, Rotation = rotation });
		}
	}
}