using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup),OrderFirst = true)]
partial struct TilesSpawnSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<TilesSpawnerComponent>();
	}
	
	[BurstCompile]
	public void OnDestroy(ref SystemState state) { }
	
	public void OnUpdate(ref SystemState state)
	{
		state.Enabled = false;
		var spawnerEntity = SystemAPI.GetSingletonEntity<TilesSpawnerComponent>();
		var aspect = SystemAPI.GetAspect<TileSpawnerAspect>(spawnerEntity);
		var ecb = new EntityCommandBuffer(Allocator.Temp);
		var offsetX = aspect.OffsetX;
		var offsetZ = aspect.OffsetZ;

		List<Tile> newTiles = new List<Tile>();
		for (int i = 0; i < aspect.Width; i++)
		{
			for (int j = 0; j < aspect.Length; j++)
			{
				var newTile = ecb.Instantiate(aspect.Entity);
				var position = GetPosition(i, j, offsetX, offsetZ, aspect.TileSize);
				ecb.SetComponent(newTile,new LocalTransform{Position = position,
					Scale = 1, Rotation = Quaternion.identity});
				newTiles.Add(new Tile(i,j,position,newTile));
			}
		}
		TilesManager.CreateTiles(newTiles,aspect.Width,aspect.Length);
		ecb.Playback(state.EntityManager);
	}

	private Vector3 GetPosition(int i, int j, float offsetX, float offsetZ,float tileSize)
	{
		return new float3(i * (tileSize + 0.1f) - offsetX, 0,
			j * (tileSize + 0.1f) - offsetZ);
	}
}
