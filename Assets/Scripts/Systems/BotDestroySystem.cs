using System;
using Unity.Collections;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SnakeElementSystem))]
public partial struct BotDestroySystem : ISystem
{
	public static Action<int> OnBotDestroyed;

	public void OnCreate(ref SystemState state) { }

	public void OnDestroy(ref SystemState state) { }

	public void OnUpdate(ref SystemState state)
	{
		if (!GameController.IsTurnFinished)
			return;
		
		var buffer = new EntityCommandBuffer(Allocator.Temp);

		foreach (var (aspect, entity) in SystemAPI.Query<BotAspect>().WithEntityAccess())
		{
			var currentTile = TilesSpawnSystem.GetTile(aspect.GetCoordinates().x, aspect.GetCoordinates().y);
			if (currentTile.Creature == Tile.CreatureType.Snake)
			{
				buffer.DestroyEntity(entity);
				OnBotDestroyed?.Invoke(1);
			}
		}
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}
}