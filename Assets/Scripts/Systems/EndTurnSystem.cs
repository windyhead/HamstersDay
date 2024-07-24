using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(MoveSystem))]

partial struct TurnSystem : ISystem
{
	public static bool IsTurnFinished = true;
	public static int CurrentTurn = 1;
	
	public void OnCreate(ref SystemState state)
	{
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		foreach (var moveComponent in SystemAPI.Query<RefRO<MoveComponent>>())
		{
			if(!moveComponent.ValueRO.MoveFinished)
				return;
		}
		IsTurnFinished = true;
		CurrentTurn ++;
	}
}

	
