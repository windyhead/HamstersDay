using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(BotEnableSystem))]
partial struct FatSystem : ISystem
{
	public static Action<int> OnPlayerFatIncreased;
	private static readonly float SizeIncreaseIndex = 0.03f;

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<PlayerComponent>();
		state.RequireForUpdate<HamsterComponent>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state) { }

	public void OnUpdate(ref SystemState state)
	{
		foreach (var (hamsterComponent, visualReference) in SystemAPI.Query<RefRW<HamsterComponent>,HamsterVisualReference>())
		{
			var nuts = hamsterComponent.ValueRO.Nuts;
			if (nuts == 0 && hamsterComponent.ValueRO.Fat >= 1)
				hamsterComponent.ValueRW.Fat --;
			else 
				hamsterComponent.ValueRW.Fat += nuts;
			
			hamsterComponent.ValueRW.Nuts = 0;
			float fatIndex = hamsterComponent.ValueRO.Fat * SizeIncreaseIndex;
			visualReference.VisualReference.Body.localScale = Vector3.one + new Vector3(fatIndex, 0, fatIndex);
			visualReference.VisualReference.LeftCheek.gameObject.SetActive(false);
			visualReference.VisualReference.RightCheek.gameObject.SetActive(false);
		}
		
		var playerQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerComponent>().WithAll<HamsterComponent>()
			.Build(ref state);
		var playerHamster = playerQuery.ToComponentDataArray<HamsterComponent>(Allocator.Temp).First();
		OnPlayerFatIncreased.Invoke(playerHamster.Fat);
	}
}