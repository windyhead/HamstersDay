using Unity.Entities;
using Unity.Mathematics;

public class Tile
{
	public int2 Coordinates;
	public float3 Center;
	public Entity Entity;
	private readonly float3 centerOffset = new (0,1.7f,0);
	
	public bool IsFinal => 	Coordinates.x + 1 == TilesSpawnSystem.Rows &&
	                        Coordinates.y + 1 == TilesSpawnSystem.Columns;

	public bool HasNut { get; private set; }
	public int Nuts { get; private set; }

	public enum TileType
	{
		Plains,
		Rocks,
		Grass
	}

	public TileType Type;
	
	public enum CreatureType
	{
		None,
		Hamster,
		Snake
	}

	public CreatureType Creature { get; private set; }

	public Tile(int rowNumber, int columnNumber, float3 transform, Entity entity)
	{
		Coordinates = new int2(rowNumber,columnNumber);
		Center = transform + centerOffset;
		Entity = entity;
	}

	public void Enter(CreatureType creatureType)
	{
		Creature = creatureType;
	}
	
	public void Exit()
	{
		Creature = CreatureType.None;
	}

	public void SetType(TileType newType)
	{
		Type = newType;
	}
	
	public void AddNut()
	{
		HasNut = true;
		Nuts++;
	}
	
	public void RemoveNuts()
	{
		HasNut = false;
		Nuts = 0;
	}

	public void Reset()
	{
		HasNut = false;
		Nuts = 0;
		Type = TileType.Plains; 
		Exit();
	}

	public bool CanBeSpawnedOn()
	{
		return Creature == CreatureType.None && Type != TileType.Rocks && !IsFinal;
	}
}
