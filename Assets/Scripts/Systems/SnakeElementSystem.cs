using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(OrientationSystem))]
partial class SnakeElementSystem : SystemBase
{
	public void OnCreate(ref SystemState state)
	{
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	protected override void OnUpdate()
	{
		if (!GameController.PlayerInputReceived)
			return;
		
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var buffer = ecbSingleton.CreateCommandBuffer(World.Unmanaged);
		var EM = EntityManager;
		
		new SnakeHeadJob{ECB = buffer,EM = EM}.Schedule();
		new BodyElementsJob{ECB = buffer,EM = EM}.Schedule();
		
		CompleteDependency();
	}
	
	public partial struct SnakeHeadJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public EntityManager EM;

		private void Execute(SnakeHeadComponent snakeHeadComponent,
			ref ActionComponent actionComponent,
			Entity entity, in RotationComponent rotationComponent)
		{
			var children = EM.GetBuffer<LinkedEntityGroup>(entity);
			var currentAction = actionComponent.ActiveAction;
			
			if (currentAction == Actions.TurnLeft || currentAction == Actions.TurnRight)
				SetEntityHierarchyEnabled(false, ECB.AsParallelWriter(), 7, children);
			else
				SetEntityHierarchyEnabled(true, ECB.AsParallelWriter(), 7, children);
		}
	}
	
	public partial struct BodyElementsJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public EntityManager EM;
		private void Execute(SnakeBodyElementComponent bodyElement,
			ref ActionComponent actionComponent,
			Entity entity, in RotationComponent rotationComponent)
		{
			if(!rotationComponent.RotationFinished)
				return;
			
			var children = EM.GetBuffer<LinkedEntityGroup>(entity);
			var previousAction = actionComponent.PreviousAction;
			
			if (previousAction == Actions.TurnLeft || previousAction == Actions.TurnRight)
				SetEntityHierarchyEnabled(false, ECB.AsParallelWriter(), 2, children);
			else
				SetEntityHierarchyEnabled(true, ECB.AsParallelWriter(), 2, children);
			
			
			if(bodyElement.Index == GameController.CurrentStage)
				return;
			
			var currentAction = actionComponent.ActiveAction;
			 if (currentAction == Actions.TurnLeft || currentAction == Actions.TurnRight) 
			 	SetEntityHierarchyEnabled(false, ECB.AsParallelWriter(), 1, children);
			 else 
			 	SetEntityHierarchyEnabled(true, ECB.AsParallelWriter(), 1, children);
		}
	}

	private static void SetEntityHierarchyEnabled(bool enabled, EntityCommandBuffer.ParallelWriter commandBuffer, int chunkIndex,DynamicBuffer<LinkedEntityGroup> children)
	{
		if (enabled)
		{
			commandBuffer.RemoveComponent<Disabled>(chunkIndex, children[chunkIndex].Value);
		}
		else
		{
			commandBuffer.AddComponent<Disabled>(chunkIndex, children[chunkIndex].Value);
		}
	}
}
