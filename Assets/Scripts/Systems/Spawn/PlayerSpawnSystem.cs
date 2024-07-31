using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(TilesSpawnSystem))]
partial struct PlayerSpawnSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<StageSpawnerComponent>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	public void OnUpdate(ref SystemState state)
	{
		state.Enabled = false;
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var buffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		var playerComponent = new PlayerComponent();
		new PlayerSpawnSystemJob() {ECB = buffer,Player =  playerComponent}.Run();
		SystemAPI.SetSingleton(playerComponent);
	}
	
	public partial struct PlayerSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public PlayerComponent Player;

		private void Execute(StageSpawnerAspect aspect)
		{
			var newHamster = ECB.Instantiate(aspect.PlayerEntity);
			ECB.AddComponent(newHamster,Player);
			ECB.SetName(newHamster,"PLAYER");
			var actionComponent = new ActionComponent() { Action = Actions.None };
			ECB.AddComponent<ActionComponent>(newHamster,actionComponent);
			var tile = TilesManager.GetTile(aspect.PlayerPosition.x, aspect.PlayerPosition.y);
			tile.Enter();
			var orientationComponent = new OrientationComponent()
			{
				CurrentOrientation = aspect.PlayerOrientation,
				CurrentTileCoordinates = tile.Coordinates
			};
			ECB.AddComponent(newHamster, orientationComponent);
			var rotation = OrientationComponent.GetRotationByOrientation(aspect.PlayerOrientation);
			ECB.SetComponent(newHamster,
				new LocalTransform { Position = tile.Center, Scale = 3, Rotation = rotation });
			ECB.AddComponent(newHamster,new MoveComponent{MoveFinished = true});
		}
	}
}