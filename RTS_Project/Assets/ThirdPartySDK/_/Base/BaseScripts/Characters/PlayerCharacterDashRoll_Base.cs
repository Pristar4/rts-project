#region Info
// -----------------------------------------------------------------------
// PlayerCharacterDashRoll_Base.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using V_AnimationSystem;
#endregion

/*
 * Player Character Base Class
 * */
public class PlayerCharacterDashRoll_Base : MonoBehaviour
{


	public void PlayWalkingAnimation(Vector3 animationDir)
	{
		unitAnimation.PlayAnim(walkUnitAnim, animationDir, .6f, null, null,
				null);
	}

	public void PlayIdleAnimation(Vector3 animationDir)
	{
		unitAnimation.PlayAnim(idleUnitAnim, animationDir, 1f, null, null,
				null);
	}

	public void PlayDodgeAnimation(Vector3 dir)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("dBareHands_Roll"), dir, 1.5f,
				null, null,
				null);
	}

	#region BaseSetup
	private V_UnitSkeleton unitSkeleton;
	private V_UnitAnimation unitAnimation;
	private UnitAnimType idleUnitAnim;
	private UnitAnimType walkUnitAnim;

	private void Start()
	{
		var bodyTransform = transform.Find("Body");
		unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint,
				mesh => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
		unitAnimation = new V_UnitAnimation(unitSkeleton);

		idleUnitAnim = UnitAnimType.GetUnitAnimType("dSwordTwoHandedBack_Idle");
		walkUnitAnim = UnitAnimType.GetUnitAnimType("dSwordTwoHandedBack_Walk");

		PlayIdleAnimation(new Vector3(0, -1));
	}

	private void Update()
	{
		unitSkeleton.Update(Time.deltaTime);
	}

	public V_UnitAnimation GetUnitAnimation()
	{
		return unitAnimation;
	}
	#endregion
}
