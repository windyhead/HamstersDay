using Unity.Entities;
using Unity.Mathematics;

public struct MoveComponent : IComponentData
{
	public float3 TargetPosition;
	public bool MoveFinished;
}
