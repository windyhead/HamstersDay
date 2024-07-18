using Unity.Entities;
using Unity.Mathematics;

public enum Orientation
{
	Up,
	Left,
	Down,
	Right
}

public struct OrientationComponent : IComponentData
{
	public float2 CurrentTile;
	public Orientation CurrentOrientation;
}
