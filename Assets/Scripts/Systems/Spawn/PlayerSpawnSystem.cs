using Unity.Burst;
using Unity.Entities;
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
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var buffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		new PlayerSpawnSystemJob() {ECB = buffer}.Run();
	}
	
	public partial struct PlayerSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;

		private void Execute(StageSpawnerAspect aspect)
		{
			var newHamster = ECB.Instantiate(aspect.HamsterEntity);
			ECB.SetName(newHamster,"Player");
			ECB.AddComponent<PlayerComponent>(newHamster);
			ECB.AddComponent(newHamster,new HamsterComponent {Fat = 1});
			var actionComponent = new ActionComponent() { CurrentAction = Actions.None };
			ECB.AddComponent<ActionComponent>(newHamster,actionComponent);
			var tile = TilesSpawnSystem.GetTile(aspect.PlayerPosition.x, aspect.PlayerPosition.y);
			tile.Enter(Tile.CreatureType.Hamster);
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
			ECB.AddComponent(newHamster,new RotationComponent(){RotationFinished = true});
			ECB.AddComponent(newHamster,new StaminaComponent(20));
		}
	}
}