using System;
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(FatSystem))]
partial class StaminaSystem : SystemBase
{
	public static Action<int> OnPlayerStaminaChanged;
	private Entity player;

	[BurstCompile]
	protected override void OnCreate()
	{
		RequireForUpdate<PlayerComponent>();
		RequireForUpdate<HamsterComponent>();
	}

	[BurstCompile]
	protected override void OnStartRunning()
	{
		player = SystemAPI.GetSingletonEntity<PlayerComponent>();
	}
	
	protected override void OnUpdate()
	{
		foreach (var hamster in SystemAPI.Query<HamsterAspect>())
		{
			hamster.CalcStamina(hamster.Fat);
			hamster.Rest();
		}

		var playerStamina = SystemAPI.GetComponent<StaminaComponent>(player).StaminaLeft;
		OnPlayerStaminaChanged?.Invoke(playerStamina);
	}
}