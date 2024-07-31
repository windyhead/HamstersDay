using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public static class TilesManager
{
	public static List<Tile> Tiles { get; private set; } = new List<Tile>();
	public static int Rows { get; private set; }
	public static int Columns { get; private set; }
	
	
	public static Tile GetRandomTile(Random random)
	{
		var tileFound = false;
		while (!tileFound)
		{
			var randomRow = random.NextInt(0, Rows);
			var randomColumn = random.NextInt(0, Columns);
			var tile = GetTile(randomRow, randomColumn);
			if (!tile.isEmpty)
				continue;
			return tile;
		}
		return null;
	}

	public static Tile GetTile(int row, int column)
	{
		foreach (var tile in Tiles)
		{
			if(tile.Coordinates.x!=row)
				continue;
			if(tile.Coordinates.y!=column)
				continue;
			return tile;
		}
		return null;
	}

	public static void CreateTiles(List<Tile> tiles,int rows, int columns)
	{
		Tiles = tiles;
		Rows = rows;
		Columns = columns;
	}

	public static bool isFinalTile(float2 coordinates) => coordinates.x + 1 == Rows && coordinates.y + 1 == Columns;
	public static int2 GetFinalTileCoordinates => new int2(Rows-1, Columns-1);
}

public class Tile
{
	public int2 Coordinates;
	public float3 Center;
	public bool isEmpty { get; private set; } = true;
	public Entity Entity;
	private readonly float3 centerOffset = new (0,1.5f,0);

	public Tile(int rowNumber, int columnNumber, float3 transform, Entity entity)
	{
		Coordinates = new int2(rowNumber,columnNumber);
		Center = transform + centerOffset;
		Entity = entity;
	}

	public void Enter()
	{
		isEmpty = false;
	}

	public void Exit()
	{
		isEmpty = true;
	}
}
