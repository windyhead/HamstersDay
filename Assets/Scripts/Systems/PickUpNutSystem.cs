using Unity.Collections;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(BotDisableSystem))]
public partial class PickUpNutSystem : SystemBase
{
	protected override void OnUpdate()
	{
		if (!GameController.IsTurnFinished)
			return;
		var buffer = new EntityCommandBuffer(Allocator.Temp);
		new PickNutJob().Run();
		
		foreach (var (nutComponent, entity) in SystemAPI.Query<NutComponent>().WithEntityAccess())
		{
			var currentTile = TilesSpawnSystem.GetTile(nutComponent.CurrentTileCoordinates.x, 
				nutComponent.CurrentTileCoordinates.y);
			if (!currentTile.HasNut)
			{
				buffer.DestroyEntity(entity);
			}
		}
		buffer.Playback(EntityManager);
		buffer.Dispose();
	}
	
	public partial struct PickNutJob : IJobEntity {
     		private void Execute(HamsterAspect hamsterAspect, HamsterVisualReference visualReference)
     		{
     			var currentTile = TilesSpawnSystem.GetTile(hamsterAspect.GetCoordinates().x, hamsterAspect.GetCoordinates().y);
     			if (currentTile.HasNut)
     			{
     				if(hamsterAspect.Nuts >= 2)
     					return;
			        var nuts = currentTile.Nuts;	
     				currentTile.RemoveNuts();
     				hamsterAspect.TakeNuts(nuts);
     				if (hamsterAspect.Nuts == 1)
     					visualReference.VisualReference.RightCheek.gameObject.SetActive(true);
     				else if (hamsterAspect.Nuts >= 2)
			        {
				        visualReference.VisualReference.RightCheek.gameObject.SetActive(true);
				        visualReference.VisualReference.LeftCheek.gameObject.SetActive(true);
			        }
		        }
     		}
     	}
}
