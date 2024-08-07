using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
[UpdateInGroup(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(MoveSystem))]

partial struct RotationSystem : ISystem
{
	public void OnCreate(ref SystemState state) { }

	public void OnDestroy(ref SystemState state) { }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		new RotationJob{Time = SystemAPI.Time.DeltaTime}.Schedule();
	}

	public partial struct RotationJob : IJobEntity
	{
		public float Time;
		private void Execute(ref LocalTransform  transform, ref OrientationComponent orientationComponent, ref RotationComponent rotationComponent)
		{
			if(rotationComponent.RotationFinished)
				return;

			var angle = math.angle(rotationComponent.TargetRotation, transform.Rotation);
			if (angle <= 0.1)
				rotationComponent.RotationFinished = true;
			
			if (!rotationComponent.RotationFinished)
				transform.Rotation = Quaternion.RotateTowards(transform.Rotation,rotationComponent.TargetRotation,120 * Time);
		}
	}
}
