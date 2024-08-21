using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct PresentationObjectSystem :ISystem
{
	public void OnCreate(ref SystemState state)
	{
		
	}

	public void OnDestroy(ref SystemState state)
	{
		
	}

	public void OnUpdate(ref SystemState state)
	{
		var buffer = new EntityCommandBuffer(Allocator.Temp);
		
		foreach (var (presentationObject,entity) in SystemAPI.Query<PresentationObject>().WithNone<AnimatorReference>().WithEntityAccess())
		{
			var newObject = GameObject.Instantiate(presentationObject.Prefab);
			newObject.name = "playerPresentation";
			buffer.AddComponent(entity,new AnimatorReference{ Animator = newObject.GetComponent<Animator>()});
			buffer.AddComponent(entity,new MeshRendererReference(){ MeshRenderers = newObject.GetComponent<MeshRendererReferenceMono>()});
		}
		
		foreach (var (animator, transform,moveComponent,entity) in SystemAPI.Query<AnimatorReference,LocalTransform,MoveComponent>().WithEntityAccess())
		{
			animator.Animator.SetBool("IsMoving",!moveComponent.MoveFinished);
			animator.Animator.transform.position = transform.Position;
			animator.Animator.transform.rotation = transform.Rotation;
		}
		
		foreach (var (meshRenderer,playerComponent,entity) in SystemAPI.Query<MeshRendererReference,PlayerComponent>().WithEntityAccess())
		{
			SetBodyPartsColor(meshRenderer, true);
			buffer.RemoveComponent<MeshRendererReference>(entity);
		}
		
		foreach (var (meshRenderer,botComponent,entity) in SystemAPI.Query<MeshRendererReference,BotComponent>().WithEntityAccess())
		{
			SetBodyPartsColor(meshRenderer, false);
			buffer.RemoveComponent<MeshRendererReference>(entity);
		}
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}

	private static void SetBodyPartsColor(MeshRendererReference meshRenderer, bool isPlayer)
	{
		foreach (var bodyPart in meshRenderer.MeshRenderers.Body)
			bodyPart.material = isPlayer ? meshRenderer.MeshRenderers.YellowMat : meshRenderer.MeshRenderers.BrownMat;
		foreach (var bodyPart in meshRenderer.MeshRenderers.Head)
			bodyPart.material = isPlayer ? meshRenderer.MeshRenderers.YellowMat : meshRenderer.MeshRenderers.BrownMat;
		foreach (var bodyPart in meshRenderer.MeshRenderers.Ears)
			bodyPart.material = isPlayer ? meshRenderer.MeshRenderers.BrownMat : meshRenderer.MeshRenderers.YellowMat;
		foreach (var bodyPart in meshRenderer.MeshRenderers.Paws)
			bodyPart.material = isPlayer ? meshRenderer.MeshRenderers.YellowMat : meshRenderer.MeshRenderers.BrownMat;
		foreach (var bodyPart in meshRenderer.MeshRenderers.Tail)
			bodyPart.material = isPlayer ? meshRenderer.MeshRenderers.BrownMat : meshRenderer.MeshRenderers.YellowMat;
	}
}
