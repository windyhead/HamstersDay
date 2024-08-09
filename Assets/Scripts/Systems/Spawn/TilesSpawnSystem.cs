using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup),OrderFirst = true)]
partial struct TilesSpawnSystem : ISystem
{
	public static int Rows { get; set; }
	public static int Columns { get; set; }

	private static List<Tile> Tiles { get; set; }
	
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<TilesSpawnerComponent>();
	}
	
	[BurstCompile]
	public void OnDestroy(ref SystemState state) { }
	
	public void OnUpdate(ref SystemState state)
	{
		var spawnerEntity = SystemAPI.GetSingletonEntity<TilesSpawnerComponent>();
		var aspect = SystemAPI.GetAspect<TileSpawnerAspect>(spawnerEntity);
		var ecb = new EntityCommandBuffer(Allocator.Temp);
		var offsetX = aspect.OffsetX;
		var offsetZ = aspect.OffsetZ;

		List<Tile> newTiles = new List<Tile>();
		for (int i = 0; i < aspect.Width; i++)
		{
			for (int j = 0; j < aspect.Length; j++)
			{
				var newTile = ecb.Instantiate(aspect.Entity);
				ecb.SetName(newTile,"Tile_"+ i + "_"+ j);
				var position = GetPosition(i, j, offsetX, offsetZ, aspect.TileSize);
				ecb.SetComponent(newTile,new LocalTransform{Position = position,
					Scale = 1, Rotation = Quaternion.identity});
				newTiles.Add(new Tile(i,j,position,newTile));
			}
		}
		CreateTiles(newTiles,aspect.Width,aspect.Length);
		ecb.Playback(state.EntityManager);
	}

	private Vector3 GetPosition(int i, int j, float offsetX, float offsetZ,float tileSize)
	{
		return new float3(i * (tileSize + 0.1f) - offsetX, 0,
			j * (tileSize + 0.1f) - offsetZ);
	}

	private static void CreateTiles(List<Tile> tiles,int rows, int columns)
	{
		Tiles = tiles;
		Rows = rows;
		Columns = columns;
	}
	
	public static Tile GetRandomTile(Random random,bool forEnvironment = false)
	{
		var tileFound = false;
		while (!tileFound)
		{
			var minRow = forEnvironment ? 1 : 0;
			var minColumn = forEnvironment ? 1 : 0;
			var maxRow = forEnvironment ? Rows -1 : Rows;
			var maxColumn = forEnvironment ? Columns -1 : Columns;
			var randomRow = random.NextInt(minRow, maxRow);
			var randomColumn = random.NextInt(minColumn, maxColumn);
			var tile = GetTile(randomRow, randomColumn);
			if (!tile.IsEmpty)
				continue;
			if(tile.IsFinal)
				continue;
			return tile;
		}
		return null;
	}
	
	public static Tile GetRandomTileOnBorders(Random random,Orientation orientation)
	{
		var minRow = 0;
		var minColumn = 0;
		var maxRow = Rows - 1;
		var maxColumn = Columns - 1;

		switch (orientation)
		{
			case Orientation.Up:
				maxColumn = 0;
				break;
			case Orientation.Left:
				minRow = Rows -1;
				break;
			case Orientation.Down:
				minColumn = Columns -1;
				break;
			case Orientation.Right:
				maxRow = 0;
				
				break;
		}
		
		var tileFound = false;
		while (!tileFound)
		{
			var randomRow = random.NextInt(minRow, maxRow);
			var randomColumn = random.NextInt(minColumn, maxColumn);
			var tile = GetTile(randomRow, randomColumn);
			if (!tile.IsEmpty)
				continue;
			if(tile.IsFinal)
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
	public static bool isFinalTile(float2 coordinates) => coordinates.x + 1 == Rows && coordinates.y + 1 == Columns;

	public static void ResetTiles()
	{
		foreach (var tile in Tiles)
		{
			tile.Reset();
		}
	}
}
