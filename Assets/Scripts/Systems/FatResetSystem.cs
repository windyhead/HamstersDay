using System;
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(BotEnableSystem))]
partial struct FatResetSystem : ISystem
{
	public void OnCreate(ref SystemState state) { }
	
	public void OnDestroy(ref SystemState state) { }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		foreach (var hamsterComponent in SystemAPI.Query<RefRW<HamsterComponent>>())
		{
			hamsterComponent.ValueRW.Fat = 0;
			hamsterComponent.ValueRW.Nuts = 0;
		}
	}
}