using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[DisableAutoCreation]
[UpdateAfter(typeof(PresentationObjectSystem))]
partial struct SnakeSpawnSystem : ISystem
{
	public static Action OnSnakeSpawned;
	public static Action OnSnakeGone;
	public static bool IsSnakeSpawned { get; private set; }
	private static bool spawnSnake;

	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<SnakeSpawnerComponent>();
		GameController.OnPopulationChanged += DetectSnakeSpawn;
	}

	private void DetectSnakeSpawn()
	{
		if (GameController.CurrentStage == 1) 
			return;
		
		var random = Random.CreateFromIndex((uint)(GameController.RandomSeed + PopulationSystem.Population));
		var randomNumber = random.NextInt(0, (TilesSpawnSystem.Rows -1) * (TilesSpawnSystem.Columns -1));
		
		if (randomNumber <= PopulationSystem.Population + 20)
			spawnSnake = true;
		
		else if (IsSnakeSpawned)
		{
			IsSnakeSpawned = false;
			OnSnakeGone?.Invoke();
		}
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	public void OnUpdate(ref SystemState state)
	{
		if(!spawnSnake)
			return;
		
		if(!IsSnakeSpawned) 
			OnSnakeSpawned?.Invoke();
		
		IsSnakeSpawned = true;
		spawnSnake = false;
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var buffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		new SnakeSpawnSystemJob() { ECB = buffer, RandomNumber = GameController.RandomSeed }.Schedule();
	}

	public partial struct SnakeSpawnSystemJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public int RandomNumber;

		private void Execute(SnakeSpawnerAspect aspect)
		{
			var snakeHead = ECB.Instantiate(aspect.HeadEntity);
			ECB.SetName(snakeHead,"SNAKEHEAD");
			
			var headComponent = new SnakeHeadComponent();
			ECB.AddComponent(snakeHead, headComponent);

			ECB.AddComponent(snakeHead, new SnakeTag());
			
			var random = Random.CreateFromIndex((uint)(RandomNumber));
			var randomComponent = new RandomComponent() { Value = random };
			ECB.AddComponent(snakeHead, randomComponent);
			
			var actionComponent = new ActionComponent() { CurrentAction = Actions.None };
			ECB.AddComponent<ActionComponent>(snakeHead, actionComponent);
			
			var randomOrientationNumber = random.NextInt(0, Enum.GetValues(typeof(Orientation)).Length);
			var orientation = (Orientation)randomOrientationNumber;
			var tile = TilesSpawnSystem.GetRandomTileOnBorders(random, orientation);
			tile.Enter(Tile.CreatureType.Snake);
			
			var orientationComponent = new OrientationComponent()
				{ CurrentOrientation = orientation, CurrentTileCoordinates = tile.Coordinates };
			ECB.AddComponent(snakeHead, orientationComponent);
			
			var rotation = OrientationComponent.GetRotationByOrientation(orientation);
			ECB.AddComponent(snakeHead, new RotationComponent() { RotationFinished = true });
			
			ECB.AddComponent(snakeHead, new MoveComponent { MoveFinished = true });
			
			ECB.SetComponent(snakeHead,
				new LocalTransform { Position = tile.Center, Scale = 3, Rotation = rotation });

			for (var i = 0; i < GameController.CurrentStage; i++)
			{
				var snakeBody = ECB.Instantiate(aspect.BodyEntity);
				ECB.SetName(snakeBody,"SnakeBody_"+ i);
				var bodyComponent = new SnakeBodyElementComponent(){Index = i};
				ECB.AddComponent(snakeBody, bodyComponent);
				
				ECB.AddComponent(snakeBody, new SnakeTag());
				
				new ActionComponent() { CurrentAction = Actions.None };
				ECB.AddComponent<ActionComponent>(snakeBody, actionComponent);
				
				ECB.AddComponent(snakeBody, orientationComponent);

				ECB.AddComponent(snakeBody, new RotationComponent() { RotationFinished = true });
			
				ECB.AddComponent(snakeBody, new MoveComponent { MoveFinished = true });
			
				ECB.SetComponent(snakeBody,
					new LocalTransform { Position = tile.Center, Scale = 3, Rotation = rotation });
			}
		}
	}

	public static void Reset()
	{
		IsSnakeSpawned = false;
	}
}