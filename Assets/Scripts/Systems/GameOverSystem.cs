using System;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TurnSystem))]
public partial struct GameOverSystem : ISystem
{
	public static Action OnGameOver;
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

		foreach (var (aspect, entity) in SystemAPI.Query<PlayerAspect>().WithEntityAccess())
		{
			var currentTile = TilesSpawnSystem.GetTile(aspect.GetCoordinates().x, aspect.GetCoordinates().y);
			if (currentTile.Creature == Tile.CreatureType.Snake)
			{
				OnGameOver?.Invoke();
			}
		}
	}
}