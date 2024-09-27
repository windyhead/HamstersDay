using Unity.Entities;

public struct HamsterComponent : IComponentData
{
	public int Fat;
	public int Nuts;

	public void Reset()
	{
		Fat = 0;
		Nuts = 0;
	}
}
