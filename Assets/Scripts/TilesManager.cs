using Unity.Entities;
using Unity.Mathematics;

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
