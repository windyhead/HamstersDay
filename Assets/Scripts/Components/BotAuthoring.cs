using Unity.Entities;
using UnityEngine;

public class BotAuthoring : MonoBehaviour
{
}

class  BotComponentBaker : Baker<BotAuthoring>
{
	public override void Bake(BotAuthoring authoring)
	{
		AddComponent(new BotComponent());
	}
}
