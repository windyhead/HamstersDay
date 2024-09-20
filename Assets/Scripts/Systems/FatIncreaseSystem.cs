using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(BotEnableSystem))]
partial struct FatIncreaseSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<HamsterComponent>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state) { }

	public void OnUpdate(ref SystemState state)
	{
		new FatJob().Schedule();
	}
	
	public partial struct FatJob : IJobEntity
	{
		private void Execute(HamsterComponent hamsterComponent)
		{
			hamsterComponent.Fat++;
		}
	}
}