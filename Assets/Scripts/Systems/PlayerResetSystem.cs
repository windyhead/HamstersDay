using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(TilesSpawnSystem))]
partial struct PlayerResetSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<PlayerComponent>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state) { }

	public void OnUpdate(ref SystemState state)
	{
		new PlayerResetJob().Run();
	}
	
	public partial struct PlayerResetJob : IJobEntity
	{
		private void Execute(PlayerAspect aspect)
		{
			aspect.ResetFat();
			aspect.ResetStamina();
		}
	}
}