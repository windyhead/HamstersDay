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

	public static Action <int> OnPlayerFatIncreased;
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<HamsterComponent>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state) { }

	public void OnUpdate(ref SystemState state)
	{
		new FatJob().Run();
		var playerQuery = new EntityQueryBuilder(Allocator.Temp).WithAspect<PlayerAspect>().WithAll<HamsterComponent>().Build(ref state);
		var playerComponent = playerQuery.ToComponentDataArray<HamsterComponent>(Allocator.Temp).First();
		OnPlayerFatIncreased.Invoke(playerComponent.Fat);
	}
	
	public partial struct FatJob : IJobEntity
	{
		private void Execute(ref HamsterComponent hamsterComponent, HamsterVisualReference visualReference)
		{
			hamsterComponent.Fat += hamsterComponent.Nuts;
			hamsterComponent.Nuts = 0;
			float fatIndex = hamsterComponent.Fat * 0.03f ;
			visualReference.VisualReference.Body.localScale = Vector3.one + new Vector3(fatIndex, 0, fatIndex);
			visualReference.VisualReference.LeftCheek.gameObject.SetActive(false);
			visualReference.VisualReference.RightCheek.gameObject.SetActive(false);
		}
	}
	public static void ResetPlayer(World world)
	{
		var playerQuery = new EntityQueryBuilder(Allocator.Temp).WithAspect<PlayerAspect>().
			WithAll<HamsterComponent>().Build(world.EntityManager);
		var hamsterComponent = playerQuery.ToComponentDataArray<HamsterComponent>(Allocator.Temp).First();
		hamsterComponent.Fat = 0;
		hamsterComponent.Nuts = 0;
		playerQuery.Dispose();
		OnPlayerFatIncreased.Invoke(hamsterComponent.Fat);
	}
}