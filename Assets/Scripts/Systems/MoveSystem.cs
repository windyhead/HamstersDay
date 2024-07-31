using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[DisableAutoCreation]
[UpdateInGroup(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(OrientationSystem))]

partial struct MoveSystem : ISystem
{
	public void OnCreate(ref SystemState state) { }

	public void OnDestroy(ref SystemState state) { }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		new MoveJob{Time = SystemAPI.Time.DeltaTime}.Schedule();
	}

	public partial struct MoveJob : IJobEntity
	{
		public float Time;
		private void Execute(ref LocalTransform  transform, ref OrientationComponent orientationComponent, ref MoveComponent moveComponent)
		{
			if(moveComponent.MoveFinished)
				return;
			
			var distance = math.distance(transform.Position, moveComponent.TargetPosition);
			if (distance <= 0.1)
				moveComponent.MoveFinished = true;
			
			var direction = math.normalize(moveComponent.TargetPosition - transform.Position);

			if (!moveComponent.MoveFinished)
				transform.Position +=  direction * 12 * Time;
		}
	}
}
