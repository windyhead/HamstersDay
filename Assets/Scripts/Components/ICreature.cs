using Unity.Mathematics;
using UnityEngine;

public interface ICreature
{
	public Actions GetAction();
	
	public void SetAction(Actions action);

	public Orientation GetCurrentOrientation();

	public Tile GetForwardTile();
	
	public bool CanMove(Tile tile);

	public void SetOrientation(Orientation orientation);

	public void SetCoordinates(int2 coordinates);

	public int2 GetCoordinates();

	public void SetTargetPosition(float3 target);

	public void SetTargetRotation(Quaternion target);
}