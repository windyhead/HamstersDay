using Unity.Collections;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(BotDestroySystem))]
public partial struct BotDisableSystem : ISystem
{
	public void OnCreate(ref SystemState state) { }

	public void OnDestroy(ref SystemState state) { }

	public void OnUpdate(ref SystemState state)
	{
		if (!GameController.IsTurnFinished)
			return;
		
		var buffer = new EntityCommandBuffer(Allocator.Temp);

		foreach (var (aspect, gameObjectReference, entity) in SystemAPI.Query<BotAspect, GameObjectReference>()
			         .WithEntityAccess())
		{
			var currentTile = TilesSpawnSystem.GetTile(aspect.GetCoordinates().x, aspect.GetCoordinates().y);
			if (aspect.OnFinalTile)
			{
				gameObjectReference.MainObject.SetActive(false);
				currentTile.Exit();
				buffer.AddComponent<DisabledTag>(entity);
				buffer.SetEnabled(entity, false);
			}
		}
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}
}
