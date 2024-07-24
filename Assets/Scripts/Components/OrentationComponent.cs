using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum Orientation
{
	Up,
	Left,
	Down,
	Right
}

public struct OrientationComponent : IComponentData
{
	public int2 CurrentTileCoordinates;
	public Orientation CurrentOrientation;
	
	public static Quaternion GetRotationByOrientation(Orientation orientation)
	{
		Quaternion rotation = Quaternion.identity;
		switch (orientation)
		{
			case Orientation.Left:
				rotation = Quaternion.LookRotation(Vector3.left);
				break;
			case Orientation.Right:
				rotation = Quaternion.LookRotation(Vector3.right);
				break;
			case Orientation.Down:
				rotation = Quaternion.LookRotation(new Vector3(0, 0, -1));
				break;
		}
		return rotation;
	}
	
	public bool GetTileAvailable(Actions action)
	{	Tile tile = null;
		switch (action)
		{
			case Actions.Move:
				tile = GetForwardTile();
				break;
			case Actions.TurnLeft:
				tile = GetLeftTile();
				break;
			case Actions.TurnRight:
				tile = GetRightTile();
				break;
		}
		if (tile == null) 
			return false;
		if (!tile.isEmpty)
			return false;
		return true;
	}

	public Tile GetForwardTile()
	{
		var tileCoordinates = CurrentTileCoordinates;
		switch (CurrentOrientation)
		{
			case Orientation.Up:
				if (CurrentTileCoordinates.y == TilesManager.Columns)
					return null;
				tileCoordinates.y = CurrentTileCoordinates.y + 1;
				break;
			case Orientation.Left:
				if (CurrentTileCoordinates.x == 0)
					return null;
				tileCoordinates.x = CurrentTileCoordinates.x - 1;
				break;
			case Orientation.Right:
				if (CurrentTileCoordinates.x == TilesManager.Rows)
					return null;
				tileCoordinates.x = CurrentTileCoordinates.x + 1;
				break;
			case Orientation.Down:
				if (CurrentTileCoordinates.y == 0)
					return null;
				tileCoordinates.y = CurrentTileCoordinates.y - 1;
				break;
		}
		var forwardTile = TilesManager.GetTile(tileCoordinates.x, tileCoordinates.y);
		return forwardTile;
	}
	
	private Tile GetLeftTile()
	{
		var tile = CurrentTileCoordinates;
		switch (CurrentOrientation)
		{
			case Orientation.Up:
				if (CurrentTileCoordinates.x == 0)
					return null;
				tile.x = CurrentTileCoordinates.x - 1;
				break;
			
			case Orientation.Left:
				if (CurrentTileCoordinates.y == 0)
					return null;
				tile.y = CurrentTileCoordinates.y - 1;
				break;
				
			case Orientation.Right:
				if (CurrentTileCoordinates.y == TilesManager.Columns)
					return null;
				tile.y = CurrentTileCoordinates.y + 1;
				break;
			
			case Orientation.Down:
				if (CurrentTileCoordinates.x == TilesManager.Rows)
					return null;
				tile.x = CurrentTileCoordinates.x + 1;
				break;
		}
		var leftTile = TilesManager.GetTile(tile.x, tile.y);
		return leftTile;
	}
	
	private Tile GetRightTile()
	{
		var tile = CurrentTileCoordinates;
		switch (CurrentOrientation)
		{
			case Orientation.Up:
				if (CurrentTileCoordinates.x == TilesManager.Rows)
					return null;
				tile.x = CurrentTileCoordinates.x + 1;
				break;
			
			case Orientation.Left:
				if (CurrentTileCoordinates.y == TilesManager.Columns)
					return null;
				tile.y = CurrentTileCoordinates.y + 1;
				break;
			
			case Orientation.Right:
				if (CurrentTileCoordinates.y == 0)
					return null;
				tile.y = CurrentTileCoordinates.y - 1;
				break;
				
			case Orientation.Down:
				if (CurrentTileCoordinates.x == 0)
					return null;
				tile.x = CurrentTileCoordinates.x - 1;
				break;
		}
		var rightTile = TilesManager.GetTile(tile.x, tile.y);
		return rightTile;
	}
}
