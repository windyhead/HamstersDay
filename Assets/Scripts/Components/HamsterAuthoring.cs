using System;
using Unity.Entities;
using UnityEngine;

public class HamsterAuthoring : MonoBehaviour
{
	public GameObject Prefab;
}

public class PresentationObject : IComponentData
{
	public GameObject Prefab;
}

[Serializable]
public class GameObjectReference : ICleanupComponentData
{
	public GameObject MainObject;
}

public class MeshRendererReference : IComponentData
{
	public MeshRendererReferenceMono MeshRenderers;

}

public class HamsterVisualReference : IComponentData
{
	public HamsterVisualReferenceMono VisualReference;
}

[Serializable]
public class AnimatorReference : ICleanupComponentData
{
	public Animator Animator;
}
public class HamsterComponentBaker : Baker<HamsterAuthoring>
{
	public override void Bake(HamsterAuthoring authoring)
	{
		PresentationObject presentationObject = new PresentationObject();
		presentationObject.Prefab = authoring.Prefab;
		AddComponentObject(presentationObject);
	}
}

