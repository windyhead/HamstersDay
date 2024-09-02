
using Unity.Collections;
using Unity.Entities;

public static class EntityUtils
{
	public static void DestroyEntitiesWithComponent<T>(World world)
	{
		var query = new EntityQueryBuilder(Allocator.Temp).WithAll<T>().Build(world.EntityManager);
		var entities = query.ToEntityArray(Allocator.TempJob);
		world.EntityManager.DestroyEntity(entities);
		entities.Dispose();
		query.Dispose();
	}
}
