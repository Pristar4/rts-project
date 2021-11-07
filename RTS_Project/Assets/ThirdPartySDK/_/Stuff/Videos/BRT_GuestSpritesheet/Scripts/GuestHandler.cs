#region Info
// -----------------------------------------------------------------------
// GuestHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using V_AnimationSystem;
#endregion
public class GuestHandler : MonoBehaviour
{

	private UnitAnimType idleUnitAnimType;
	private V_UnitAnimation unitAnimation;

	private V_UnitSkeleton unitSkeleton;


	private void Start()
	{
		var bodyTransform = transform.Find("Body");
		unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint,
				mesh => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
		unitAnimation = new V_UnitAnimation(unitSkeleton);

		idleUnitAnimType = UnitAnimType.GetUnitAnimType("dBareHands_Idle");

		unitAnimation.PlayAnim(idleUnitAnimType, new Vector3(0, -1), 1f, null,
				null,
				null);
	}

	private void Update()
	{
		unitSkeleton.Update(Time.deltaTime);
	}
}
