using Unity.Entities;
using Unity.Mathematics;

public class Tile
{
	public int2 Coordinates;
	public float3 Center;
	public bool IsEmpty { get; private set; } = true;
	public Entity Entity;
	private readonly float3 centerOffset = new (0,1.5f,0);
	public bool IsFinal { get; private set; }
	public bool HasSnake { get; private set; }
	public enum TileType
	{
		Plains,
		Rocks,
		Grass
	}

	public TileType Type;

	public Tile(int rowNumber, int columnNumber, float3 transform, Entity entity)
	{
		Coordinates = new int2(rowNumber,columnNumber);
		Center = transform + centerOffset;
		Entity = entity;
		IsFinal = rowNumber + 1 == TilesSpawnSystem.Rows &&
		          columnNumber + 1 == TilesSpawnSystem.Columns;
	}

	public void Enter()
	{
		IsEmpty = false;
	}
	
	public void SnakeEnter()
	{
		HasSnake = true;
	}
	
	public void Exit()
	{
		IsEmpty = true;
		if (HasSnake)
			HasSnake = false;
	}

	public void SetType(TileType newType)
	{
		Type = newType;
	}

	public void Reset()
	{
		if(Type != TileType.Rocks)
			Exit();
	}
}
