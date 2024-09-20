using System.Linq;
using Unity.Collections;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SnakeDecisionSystem))]
partial class OrientationSystem : SystemBase
{
	private EntityQuery hamsterQuery;
	protected override void OnUpdate()
	{ 
		if (!GameController.PlayerInputReceived)
			return;

		hamsterQuery = SystemAPI.QueryBuilder().WithAspect<HamsterAspect>().Build();
		var hamsterArray = hamsterQuery.ToEntityArray(Allocator.Temp);
		var orderedByFat =	hamsterArray.OrderBy(x=>SystemAPI.GetAspect<HamsterAspect>(x).Fat);
		foreach (var entity in orderedByFat)
		{
			var aspect = SystemAPI.GetAspect<HamsterAspect>(entity);
			
			var action = aspect.GetAction();
			if (action == Actions.None)
				continue;
			
			ApplyAction(aspect, action);
		}
		hamsterArray.Dispose();
		
		new SnakeHeadOrientationJob().Schedule();
		new SnakeBodyOrientationJob().Schedule();
	}

	private static void ApplyAction(HamsterAspect aspect, Actions action)
	{
		aspect.SetAction(Actions.None);

		switch (action)
		{
			case Actions.Move:
			{
				var newTile = aspect.GetForwardTile();
				if (!aspect.CanMove(newTile))
					return;
			
				var oldTile = TilesSpawnSystem.GetTile(aspect.GetCoordinates().x,
					aspect.GetCoordinates().y);
				oldTile.Exit();
			
				newTile.Enter(Tile.CreatureType.Hamster);
				aspect.SetCoordinates(newTile.Coordinates);
				aspect.SetTargetPosition(newTile.Center);
				break;
			}
			case Actions.TurnLeft:
			{
				TurnLeft(aspect);
				break;
			}
			
			case Actions.TurnRight:
			{
				TurnRight(aspect);
				break;
			}
		}
	}

	public partial struct SnakeHeadOrientationJob : IJobEntity
	{
		private void Execute(SnakeHeadAspect headAspect)
		{
			var action = headAspect.GetAction();
			if (action == Actions.None)
				return;

			headAspect.SetAction(Actions.None);
			headAspect.SetActiveAction(action);

			switch (action)
			{
				case Actions.Move:
				{
					var newTile = headAspect.GetForwardTile();
					var oldTile = TilesSpawnSystem.GetTile(headAspect.GetCoordinates().x,
						headAspect.GetCoordinates().y);
					oldTile.Exit();
					newTile.Enter(Tile.CreatureType.Snake);
					headAspect.SetCoordinates(newTile.Coordinates);
					headAspect.SetTargetPosition(newTile.Center);
					break;
				}
				case Actions.TurnLeft:
				{
					TurnLeft(headAspect);
					break;
				}

				case Actions.TurnRight:
				{
					TurnRight(headAspect);
					break;
				}
			}
		}
	}

	public partial struct SnakeBodyOrientationJob : IJobEntity
	{
		private void Execute(SnakeBodyAspect aspect)
		{
			var action = aspect.GetAction();
			if (action == Actions.None)
				return;
			
			aspect.SetAction(Actions.None);
			aspect.SetActiveAction(action);

			switch (action)
			{
				case Actions.Move:
				{
					var newTile = aspect.GetForwardTile();
					var oldTile = TilesSpawnSystem.GetTile(aspect.GetCoordinates().x, 
						aspect.GetCoordinates().y);
					oldTile.Exit();
					newTile.Enter(Tile.CreatureType.Snake);
					aspect.SetCoordinates(newTile.Coordinates);
					aspect.SetTargetPosition(newTile.Center);
					break;
				}
				case Actions.TurnLeft:
				{
					TurnLeft(aspect);
					break;
				}
    
				case Actions.TurnRight:
				{
					TurnRight(aspect);
					break;
				}
			}
		}
	}
	
	private static void TurnRight(ICreature aspect)
	{
		var newOrientation = Orientation.Right;
		if (aspect.GetCurrentOrientation() != Orientation.Up)
			newOrientation = aspect.GetCurrentOrientation() - 1;
		var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
		aspect.SetTargetRotation(newRotation);
		aspect.SetOrientation(newOrientation);
	}

	private static void TurnLeft(ICreature aspect)
	{
		var newOrientation = Orientation.Up;
		if (aspect.GetCurrentOrientation() != Orientation.Right)
			newOrientation = aspect.GetCurrentOrientation() + 1;
		var newRotation = OrientationComponent.GetRotationByOrientation(newOrientation);
		aspect.SetTargetRotation(newRotation);
		aspect.SetOrientation(newOrientation);
	}
	
}