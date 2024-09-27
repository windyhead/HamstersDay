using Unity.Entities;

public enum Actions
{
	None,
	Move,
	TurnLeft,
	TurnRight,
	Rest
}

public struct ActionComponent : IComponentData
{
	public Actions PreviousAction;
	public Actions CurrentAction;
	public Actions ActiveAction;
}
