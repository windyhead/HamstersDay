using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = false)]
[UpdateAfter(typeof(TilesSpawnSystem))]
partial struct BotSpawnSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<HamsterSpawnerComponent>();
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
		new BotSpawnSystemJob() { ECB = buffer}.Schedule();
	}

	public partial struct BotSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;

		private void Execute(HamsterSpawnerAspect aspect)
		{
			for (int i = 0; i < aspect.Count; i++)
			{
				var newHamster = ECB.Instantiate(aspect.Entity);
				var random = Random.CreateFromIndex((uint)i);
				var randomComponent = new RandomComponent() { Value = random };
				ECB.AddComponent(newHamster, randomComponent);
				ECB.AddComponent(newHamster,new BotComponent());
				var actionComponent = new ActionComponent() { Action = Actions.None };
				ECB.AddComponent<ActionComponent>(newHamster,actionComponent);
				var randomOrientationNumber = random.NextInt(0, Enum.GetValues(typeof(Orientation)).Length);
				var tile = TilesManager.GetRandomTile(random);
				tile.Enter();
				var orientation = (Orientation)randomOrientationNumber;
				var orientationComponent = new OrientationComponent()
					{ CurrentOrientation = orientation, CurrentTileCoordinates = new int2(tile.RowNumber, tile.ColumnNumber) };
				ECB.AddComponent(newHamster, orientationComponent);
				var rotation = OrientationComponent.GetRotationByOrientation(orientation);
				ECB.SetComponent(newHamster,
					new LocalTransform { Position = tile.Center, Scale = 2, Rotation = rotation });
				ECB.AddComponent(newHamster,new MoveComponent{MoveFinished = true});
			}
		}
	}
}