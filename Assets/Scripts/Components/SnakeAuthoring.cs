using Unity.Entities;
using UnityEngine;

public class SnakeAuthoring : MonoBehaviour
{
}

class  SnakeHeadComponentBaker : Baker<SnakeAuthoring>
{
	public override void Bake(SnakeAuthoring authoring)
	{
		AddComponent(new SnakeHeadComponent());
	}
}
