using Unity.Burst;
using Unity.Entities;

partial struct NewISystemScript : ISystem
{
	public void OnCreate(ref SystemState state) { }

	public void OnDestroy(ref SystemState state) { }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		foreach (RefRO<WheelComponent> wheel in SystemAPI.Query<RefRO<WheelComponent>>())
		{
			Spin(ref state, wheel);
		}
	}

	private void Spin(ref SystemState state, RefRO<WheelComponent> wheel)
	{
	}
}
