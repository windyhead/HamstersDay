using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(PlayerSpawnSystem))]
partial struct StageSpawnSystem : ISystem
{
	private static bool inCelestialWorld;
	
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<StageSpawnerComponent>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state) { }

	public void OnUpdate(ref SystemState state)
	{
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var buffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		new HouseSpawnSystemJob(){ECB = buffer}.Schedule();
		new StoneSpawnSystemJob(){ECB = buffer,RandomNumber = SystemsController.RandomSeed}.Schedule();
		new FlowersSpawnSystemJob(){ECB = buffer,RandomNumber = SystemsController.RandomSeed}.Schedule();
		new NutsSpawnSystemJob(){ECB = buffer,RandomNumber = SystemsController.RandomSeed}.Schedule();
	}
	
	public partial struct HouseSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		
		private void Execute(StageSpawnerAspect aspect)
		{
			var house = ECB.Instantiate(aspect.HouseEntity);
			ECB.SetName(house,"HOUSE");
			ECB.AddComponent(house, new TerrainTag());
			var tile = TilesSpawnSystem.GetTile(aspect.HousePosition.x, aspect.HousePosition.y);
			var orientationComponent = new OrientationComponent()
			{
				CurrentOrientation = aspect.HouseOrientation,
				CurrentTileCoordinates = tile.Coordinates
			};
			ECB.AddComponent(house, orientationComponent);
			var rotation = OrientationComponent.GetRotationByOrientation(aspect.HouseOrientation);
			ECB.SetComponent(house,
				new LocalTransform { Position = tile.Center + new float3(0,13,0), Scale = 1, Rotation = rotation });
		}
	}
	
	public partial struct StoneSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public int RandomNumber; 
		
		private void Execute(StageSpawnerAspect aspect)
		{
			for (int i = 0; i < aspect.StoneCount; i++)
			{
				var stone = ECB.Instantiate(aspect.StoneEntity);
				ECB.SetName(stone, "STONE");
				ECB.AddComponent(stone, new TerrainTag());
				var random = Random.CreateFromIndex((uint)(RandomNumber + i));
				var tile = TilesSpawnSystem.GetRandomTile(random,true);
				tile.SetType(Tile.TileType.Rocks);
				var randomOrientationNumber = random.NextInt(0, Enum.GetValues(typeof(Orientation)).Length);
				var orientation = (Orientation)randomOrientationNumber;
				var rotation = OrientationComponent.GetRotationByOrientation(orientation);
				ECB.SetComponent(stone,
					new LocalTransform { Position = tile.Center, Scale = 1, Rotation = rotation });
			}
		}
	}
	
	public partial struct FlowersSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public int RandomNumber; 
		
		private void Execute(StageSpawnerAspect aspect)
		{
			for (int i = 0; i < aspect.FlowersCount; i++)
			{
				var flowers = ECB.Instantiate(aspect.FlowerEntity);
				ECB.SetName(flowers, "FLOWERS");
				ECB.AddComponent(flowers, new TerrainTag());
				var random = Random.CreateFromIndex((uint)(RandomNumber + i));
				var tile = TilesSpawnSystem.GetRandomTile(random,true);
				tile.SetType(Tile.TileType.Grass);
				var randomOrientationNumber = random.NextInt(0, Enum.GetValues(typeof(Orientation)).Length);
				var orientation = (Orientation)randomOrientationNumber;
				var rotation = OrientationComponent.GetRotationByOrientation(orientation);
				ECB.SetComponent(flowers,
					new LocalTransform { Position = tile.Center, Scale = 1, Rotation = rotation });
			}
		}
	}
	
	public partial struct NutsSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public int RandomNumber; 
		
		private void Execute(StageSpawnerAspect aspect)
		{
			for (int i = 0; i < 5; i++)
			{
				var nut = ECB.Instantiate(aspect.NutEntity);
				ECB.SetName(nut, "NUT");
				var random = Random.CreateFromIndex((uint)(RandomNumber + i));
				var tile = TilesSpawnSystem.GetRandomTile(random);
				ECB.AddComponent(nut, new NutComponent(){CurrentTileCoordinates = tile.Coordinates});
				tile.SetType(Tile.TileType.Plains);
				tile.AddNut();
				var randomOrientationNumber = random.NextInt(0, Enum.GetValues(typeof(Orientation)).Length);
				var orientation = (Orientation)randomOrientationNumber;
				var rotation = OrientationComponent.GetRotationByOrientation(orientation);
				ECB.SetComponent(nut,
					new LocalTransform { Position = tile.Center, Scale = 1, Rotation = rotation });
			}
		}
	}
}