using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(StageSpawnSystem))]
partial struct BotEnableSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<BotComponent>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state) { }

	public void OnUpdate(ref SystemState state)
	{
		var buffer = new EntityCommandBuffer(Allocator.Temp);
		foreach (var (disabled, gameObjectReference,entity) in SystemAPI.Query<Disabled,GameObjectReference>().WithEntityAccess())
		{
			buffer.RemoveComponent<Disabled>(entity);
			gameObjectReference.MainObject.SetActive(true);
		}
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}
}