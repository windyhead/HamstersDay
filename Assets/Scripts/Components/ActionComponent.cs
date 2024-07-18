using Unity.Entities;

public enum Actions
{
	None,
	Move,
	TurnLeft,
	TurnRight
}

public struct ActionComponent : IComponentData
{
	public Actions Action;
}
