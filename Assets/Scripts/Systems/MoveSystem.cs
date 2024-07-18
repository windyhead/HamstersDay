using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(TransformSystemGroup))]
partial struct MoveSystem : ISystem
{
	public void OnCreate(ref SystemState state) { }

	public void OnDestroy(ref SystemState state) { }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		new PlayerMoveJob().Schedule();
	}

	public partial struct PlayerMoveJob : IJobEntity
	{
		private void Execute(ref LocalTransform  transform,ref ActionComponent actionComponent)
		{
			if (actionComponent.Action == Actions.Move)
			{
				transform = transform.Translate(Vector3.forward * 1f);
				actionComponent.Action = Actions.None;
			}
		}
	}


}
