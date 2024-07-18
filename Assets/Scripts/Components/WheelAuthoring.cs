using Unity.Entities;
using UnityEngine;

public class WheelAuthoring : MonoBehaviour
{
	public int Speed;
}

class WheelComponentBaker : Baker<WheelAuthoring>
{
	public override void Bake(WheelAuthoring authoring)
	{
		AddComponent(new WheelComponent
		{
			Speed = authoring.Speed
		});
	}
}
