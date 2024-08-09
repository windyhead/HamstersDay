using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct SnakeSpawnerAspect :IAspect
{
    private readonly RefRW<SnakeSpawnerComponent> snakeSpawner;
    public Entity HeadEntity => snakeSpawner.ValueRW.SnakeHead;
    public Entity BodyEntity => snakeSpawner.ValueRW.SnakeBodyElement;
    public Entity TailEntity => snakeSpawner.ValueRW.SnakeTail;
}
