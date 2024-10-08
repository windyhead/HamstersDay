using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
[UpdateAfter(typeof(PlayerSpawnSystem))]
public partial struct AnimationSystem :ISystem
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
		foreach (var (animator, transform,moveComponent,entity) in 
		         SystemAPI.Query<AnimatorReference,LocalTransform,MoveComponent>().WithEntityAccess())
		{
			animator.Animator.SetBool("IsMoving",!moveComponent.MoveFinished);
			animator.Animator.transform.position = transform.Position;
			animator.Animator.transform.rotation = transform.Rotation;
		}
		
		foreach (var (animator,entity) in 
		         SystemAPI.Query<AnimatorReference>().WithNone<PresentationObject,LocalTransform>().WithEntityAccess())
		{
			Object.Destroy(animator.Animator.gameObject);
			buffer.RemoveComponent<AnimatorReference>(entity);
		}
		
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}
}