using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = false)]
[UpdateAfter(typeof(StageSpawnSystem))]
partial struct BotSpawnSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<StageSpawnerComponent>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	public void OnUpdate(ref SystemState state)
	{
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var buffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		var existingBotsQuery = new EntityQueryBuilder(Allocator.Temp).WithAspect<BotAspect>().Build(ref state);
		var existingBotsCount = existingBotsQuery.CalculateEntityCount();
		new BotSpawnSystemJob() { ECB = buffer,RandomNumber = SystemsController.RandomSeed,
			BotCount = existingBotsCount}.Schedule();
	}

	public partial struct BotSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public int RandomNumber;
		public int BotCount;
		private void Execute(StageSpawnerAspect aspect)
		{
			for (int i = 0; i < PopulationSystem.Population - BotCount ; i++)
			{
				var newHamster = ECB.Instantiate(aspect.HamsterEntity);
				ECB.SetName(newHamster,"BotHamster_"+ i);
				var random = Random.CreateFromIndex((uint)(i + RandomNumber));
				var randomComponent = new RandomComponent() { Value = random };
				ECB.AddComponent(newHamster, randomComponent);
				ECB.AddComponent(newHamster,new HamsterComponent {Fat = 1});
				ECB.AddComponent(newHamster,new BotComponent());
				var actionComponent = new ActionComponent() { CurrentAction = Actions.None };
				ECB.AddComponent<ActionComponent>(newHamster,actionComponent);
				var randomOrientationNumber = random.NextInt(0, Enum.GetValues(typeof(Orientation)).Length);
				var tile = TilesSpawnSystem.GetRandomTile(random, forBots: true);
				tile.Enter(Tile.CreatureType.Hamster);
				var orientation = (Orientation)randomOrientationNumber;
				var orientationComponent = new OrientationComponent()
					{ CurrentOrientation = orientation, CurrentTileCoordinates = tile.Coordinates };
				ECB.AddComponent(newHamster, orientationComponent);
				ECB.AddComponent(newHamster, new BotComponent());
				var rotation = OrientationComponent.GetRotationByOrientation(orientation);
				ECB.SetComponent(newHamster,
					new LocalTransform { Position = tile.Center, Scale = 3, Rotation = rotation });
				ECB.AddComponent(newHamster,new MoveComponent{MoveFinished = true});
				ECB.AddComponent(newHamster,new RotationComponent(){RotationFinished = true});
			}
		}
	}
}