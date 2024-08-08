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
		new HouseSpawnSystemJob(){ECB = buffer}.Run();
		new StoneSpawnSystemJob(){ECB = buffer,RandomNumber = GameController.RandomSeed}.Run();
	}
	
	public partial struct HouseSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		
		private void Execute(StageSpawnerAspect aspect)
		{
			var house = ECB.Instantiate(aspect.HouseEntity);
			ECB.SetName(house,"HOUSE");
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
				var random = Random.CreateFromIndex((uint)(RandomNumber + i));
				var tile = TilesSpawnSystem.GetRandomTile(random);
				tile.Enter();
				var randomOrientationNumber = random.NextInt(0, Enum.GetValues(typeof(Orientation)).Length);
				var orientation = (Orientation)randomOrientationNumber;
				var rotation = OrientationComponent.GetRotationByOrientation(orientation);
				ECB.SetComponent(stone,
					new LocalTransform { Position = tile.Center, Scale = 1, Rotation = rotation });
			}
		}
	}
}