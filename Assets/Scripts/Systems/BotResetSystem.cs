using System;
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(FatIncreaseSystem))]
partial struct BotResetSystem : ISystem
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
		new ResetJob().Schedule();
	}
	
	public partial struct ResetJob : IJobEntity
	{
		private void Execute(BotAspect aspect)
		{
			aspect.SetAction(Actions.None);
			var random = aspect.Random;
			var randomOrientationNumber = random.NextInt(0, Enum.GetValues(typeof(Orientation)).Length);
			var orientation = (Orientation)randomOrientationNumber;
			var tile = TilesSpawnSystem.GetRandomTile(random);
			tile.Enter(Tile.CreatureType.Hamster);
			aspect.SetNewOrientation(orientation,tile);
			aspect.SetTransform(tile);
		}
	}
}