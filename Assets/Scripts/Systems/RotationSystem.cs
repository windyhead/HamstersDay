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
	private static readonly float NormalRotationSpeed = 130f;
	private static readonly float AlarmRotationSpeed = 260f;
	private static readonly float RotationMargin = 0.01f;
	public void OnCreate(ref SystemState state) { }

	public void OnDestroy(ref SystemState state) { }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		new SnakeBodyRotationJob().Schedule();
		new SnakeHeadRotationJob{Time = SystemAPI.Time.DeltaTime}.Schedule();
		new RotationJob{Time = SystemAPI.Time.DeltaTime}.Schedule();
	}

	public partial struct RotationJob : IJobEntity
	{
		public float Time;
		private void Execute(in HamsterComponent hamsterComponent,ref LocalTransform  transform, ref OrientationComponent orientationComponent, ref RotationComponent rotationComponent)
		{
			if(rotationComponent.RotationFinished)
				return;

			var angle = math.angle(rotationComponent.TargetRotation, transform.Rotation);
			if (angle <= RotationMargin)
				rotationComponent.RotationFinished = true;
			
			var speed = SnakeSpawnSystem.IsSnakeSpawned ? AlarmRotationSpeed : NormalRotationSpeed;
			if (!rotationComponent.RotationFinished)
				transform.Rotation = Quaternion.RotateTowards(transform.Rotation,rotationComponent.TargetRotation,speed * Time);
		}
	}
	
	public partial struct SnakeBodyRotationJob : IJobEntity
	{
		private void Execute(in SnakeBodyElementComponent snakeElement, ref LocalTransform  transform, ref OrientationComponent orientationComponent, ref RotationComponent rotationComponent)
		{
			if(rotationComponent.RotationFinished)
				return;
			
			transform.Rotation = rotationComponent.TargetRotation;
			rotationComponent.RotationFinished = true;
		}
	}
	
	public partial struct SnakeHeadRotationJob : IJobEntity
	{
		public float Time;
		private void Execute(in SnakeHeadComponent snakeHeadComponent, ref LocalTransform  transform, ref OrientationComponent orientationComponent, ref RotationComponent rotationComponent)
		{
			if(rotationComponent.RotationFinished)
				return;

			var angle = math.angle(rotationComponent.TargetRotation, transform.Rotation);
			if (angle <= RotationMargin)
				rotationComponent.RotationFinished = true;
			
			if (!rotationComponent.RotationFinished)
				transform.Rotation = Quaternion.RotateTowards(transform.Rotation,rotationComponent.TargetRotation,300 * Time);
		}
	}
}
