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

	public Tile(int rowNumber, int columnNumber, float3 transform, Entity entity)
	{
		Coordinates = new int2(rowNumber,columnNumber);
		Center = transform + centerOffset;
		Entity = entity;
		IsFinal = Coordinates.x + 1 == TilesSpawnSystem.Rows &&
		          Coordinates.y + 1 == TilesSpawnSystem.Columns;
	}

	public void Enter()
	{
		IsEmpty = false;
	}
	
	public void Exit()
	{
		IsEmpty = true;
	}
}
