using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(PlayerSpawnSystem))]
partial struct WheelSpawnSystem : ISystem
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
		state.Enabled = false;
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var buffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		new WheelSpawnSystemJob() {ECB = buffer,}.Run();
		
	}
	
	public partial struct WheelSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		
		private void Execute(StageSpawnerAspect aspect)
		{
			var wheel = ECB.Instantiate(aspect.WheelEntity);
			ECB.SetName(wheel,"Wheel");
			var tile = TilesManager.GetTile(aspect.WheelPosition.x, aspect.WheelPosition.y);
			tile.Enter();
			var orientationComponent = new OrientationComponent()
			{
				CurrentOrientation = aspect.WhellOrientation,
				CurrentTileCoordinates = tile.Coordinates
			};
			ECB.AddComponent(wheel, orientationComponent);
			var rotation = OrientationComponent.GetRotationByOrientation(aspect.PlayerOrientation);
			ECB.SetComponent(wheel,
				new LocalTransform { Position = tile.Center + new float3(0,10,0), Scale = 2, Rotation = rotation });
		}
	}
}