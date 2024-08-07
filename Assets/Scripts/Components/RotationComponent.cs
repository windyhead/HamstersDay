using Unity.Entities;
using UnityEngine;

public struct RotationComponent: IComponentData
{
	public Quaternion TargetRotation;
	public bool RotationFinished;
}
