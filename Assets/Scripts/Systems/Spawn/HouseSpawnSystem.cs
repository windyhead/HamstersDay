using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(PlayerSpawnSystem))]
partial struct HouseSpawnSystem : ISystem
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
		new HouseSpawnSystemJob() {ECB = buffer,}.Run();
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
}