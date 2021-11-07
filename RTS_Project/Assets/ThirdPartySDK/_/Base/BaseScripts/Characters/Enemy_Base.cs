#region Info
// -----------------------------------------------------------------------
// Enemy_Base.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
using V_AnimationSystem;
#endregion

/*
 * Enemy Base Class
 * */
public class Enemy_Base : MonoBehaviour
{


	public void PlayMoveAnim(Vector3 moveDir)
	{
		animatedWalker.SetMoveVector(moveDir);
	}

	public void PlayIdleAnim()
	{
		animatedWalker.SetMoveVector(Vector3.zero);
	}

	public bool IsPlayingPunchAnimation()
	{
		return unitAnimation.GetActiveAnimType().GetName() ==
		       "dBareHands_PunchQuick";
	}

	public void PlayPunchAnimation(Vector3 dir, Action<Vector3> onHit,
	                               Action onAnimComplete)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("dBareHands_PunchQuick"), dir, 1f,
				unitAnim2 =>
				{
					if (onAnimComplete != null) onAnimComplete();
				}, trigger =>
				{
					// HIT = HandR
					// HIT2 = HandL
					var hitBodyPartName = trigger == "HIT" ? "HandR" : "HandL";
					var impactPosition
							= unitSkeleton.GetBodyPartPosition(hitBodyPartName);
					if (onHit != null) onHit(impactPosition);
				}, null);
	}

	public void PlayAttackAnimation(Vector3 dir, Action onHit,
	                                Action onAnimComplete)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("dBareHands_Punch"), dir, 1f,
				unitAnim2
						=>
				{
					if (onAnimComplete != null) onAnimComplete();
				}, trigger =>
				{
					if (onHit != null) onHit();
				}, null);
	}

	public void PlayHitAnimation(Vector3 dir, Action onAnimComplete)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("dBareHands_Hit"),
				dir, 1f, unitAnim2 =>
				{
					if (onAnimComplete != null) onAnimComplete();
				}, trigger => { }, null);
	}

	public void PlayDodgeAnimation(Vector3 dir)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("dSwordShield_Roll"), dir, 1f,
				null, null,
				null);
	}

	public Vector3 GetHandLPosition()
	{
		return unitSkeleton.GetBodyPartPosition("HandL");
	}

	#region BaseSetup
	private V_UnitSkeleton unitSkeleton;
	private V_UnitAnimation unitAnimation;
	private AnimatedWalker animatedWalker;

	private void Start()
	{
		var bodyTransform = transform.Find("Body");
		unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint,
				mesh => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
		unitAnimation = new V_UnitAnimation(unitSkeleton);

		var idleUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Idle");
		var walkUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Walk");
		var hitUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Hit");

		animatedWalker
				= new AnimatedWalker(unitAnimation, idleUnitAnim, walkUnitAnim,
						1f, 1f);
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
