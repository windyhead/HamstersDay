using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[UpdateAfter(typeof(TilesSpawnSystem))]
partial struct HamsterSpawnSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<HamsterSpawnerComponent>();
	}
	
	[BurstCompile]
	public void OnDestroy(ref SystemState state) { }
	
	public void OnUpdate(ref SystemState state)
	{
		state.Enabled = false;
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		new HamsterSpawnSystemJob() { ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged) }.Run();
	}
	
	[BurstCompile]
	public partial struct HamsterSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;

		private void Execute(HamsterSpawnerAspect aspect)
		{
			for (int i = 0; i < aspect.Count; i++)
			{
				var newHamster = ECB.Instantiate(aspect.Entity);
				var random = Random.CreateFromIndex((uint)i);
				var rand = new RandomComponent() { Value = random };
				ECB.AddComponent(newHamster,rand);
				var tile = TilesManager.GetRandomTile(random);
				tile.Enter();
				ECB.SetComponent(newHamster,new LocalTransform{Position = tile.Center, Scale = 1, Rotation = Quaternion.identity});
			}
		}
	}
}
