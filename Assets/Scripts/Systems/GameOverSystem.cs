using System;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TurnSystem))]
public partial struct GameOverSystem : ISystem
{
	public static Action OnGameOver;
	private static bool gameOver;
	public void OnCreate(ref SystemState state)
	{
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	public void OnUpdate(ref SystemState state)
	{
		if (gameOver || !GameController.IsTurnFinished)
			return;

		foreach (var aspect in SystemAPI.Query<PlayerAspect>())
		{
			var currentTile = TilesSpawnSystem.GetTile(aspect.GetCoordinates().x, aspect.GetCoordinates().y);
			if (currentTile.Creature == Tile.CreatureType.Snake)
			{
				gameOver = true;
				OnGameOver?.Invoke();
			}
		}
	}
	public static void Reset()
	{
		gameOver = false;
	}
}