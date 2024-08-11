using System;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(OrientationSystem))]
public partial struct BotDestroySystem : ISystem
{
	public static Action<int> OnBorDestroyed;
	public void OnCreate(ref SystemState state)
	{
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	public void OnUpdate(ref SystemState state)
	{
		if (!GameController.IsTurnFinished)
			return;
		
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var buffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

		foreach (var (aspect, entity) in SystemAPI.Query<BotAspect>().WithEntityAccess())
		{
			var currentTile = TilesSpawnSystem.GetTile(aspect.Coordinates.x, aspect.Coordinates.y);
			if (aspect.OnFinalTile)
			{
				currentTile.Exit();
				buffer.DestroyEntity(entity);
			}

			if (currentTile.HasSnake)
			{
				buffer.DestroyEntity(entity);
				OnBorDestroyed?.Invoke(1);
			}
		}
	}
}