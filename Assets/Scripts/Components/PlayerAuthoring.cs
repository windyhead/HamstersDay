using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
}

class  PlayerComponentBaker : Baker<PlayerAuthoring>
{
	public override void Bake(PlayerAuthoring authoring)
	{
		AddComponent(new PlayerComponent(){ });
	}
}
