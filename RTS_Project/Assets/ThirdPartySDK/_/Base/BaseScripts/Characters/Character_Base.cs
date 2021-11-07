#region Info
// -----------------------------------------------------------------------
// Character_Base.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
using V_AnimationSystem;
#endregion

/*
 * Character Base Class
 * */
public class Character_Base : MonoBehaviour, ICharacterAnims, IGetPosition
{


	public void PlayMoveAnim(Vector3 moveDir)
	{
		animatedWalker.SetMoveVector(moveDir);
	}

	public void PlayIdleAnim()
	{
		animatedWalker.SetMoveVector(Vector3.zero);
	}

	public void PlayJumpAnim(Vector3 moveDir)
	{
		if (moveDir.x >= 0)
			unitAnimation.PlayAnim(
					UnitAnim.GetUnitAnim("dBareHands_JumpRight"));
		else
			unitAnimation.PlayAnim(UnitAnim.GetUnitAnim("dBareHands_JumpLeft"));
	}

	public bool IsPlayingPunchAnimation()
	{
		return unitAnimation.GetActiveAnimType().GetName() ==
		       "dBareHands_PunchQuick";
	}

	public bool IsPlayingKickAnimation()
	{
		return unitAnimation.GetActiveAnimType().GetName() ==
		       "dBareHands_KickQuick";
	}

	public void PlayAttackAnimation(Vector3 dir, Action onAnimComplete)
	{
		lastAttackWasPunch = !lastAttackWasPunch;
		if (lastAttackWasPunch)
			PlayKickAnimation(dir, hitPosition =>
			{
				var attackAngle = UtilsClass.GetAngleFromVectorFloat(dir);
				hitPosition += UtilsClass.GetVectorFromAngle((int)attackAngle) *
				               4f;
				var impactEffect = Instantiate(GameAssets.i.pfImpactEffect,
						hitPosition,
						Quaternion.identity);
				impactEffect.eulerAngles = new Vector3(0, 0, attackAngle - 90);
			}, onAnimComplete);
		else
			PlayPunchAnimation(dir, hitPosition =>
			{
				var attackAngle = UtilsClass.GetAngleFromVectorFloat(dir);
				hitPosition += UtilsClass.GetVectorFromAngle((int)attackAngle) *
				               4f;
				var impactEffect = Instantiate(GameAssets.i.pfImpactEffect,
						hitPosition,
						Quaternion.identity);
				impactEffect.eulerAngles = new Vector3(0, 0, attackAngle - 90);
			}, onAnimComplete);
	}

	public void PlayPunchAnimation(Vector3 dir, Action<Vector3> onHit,
	                               Action onAnimComplete,
	                               float frameRateMod = 1f)
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

	public void PlayKickAnimation(Vector3 dir, Action<Vector3> onHit,
	                              Action onAnimComplete)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("dBareHands_KickQuick"), dir, 1f,
				unitAnim2
						=>
				{
					if (onAnimComplete != null) onAnimComplete();
				}, trigger =>
				{
					// HIT = FootL
					// HIT2 = FootR
					var hitBodyPartName = trigger == "HIT" ? "FootL" : "FootR";
					var impactPosition
							= unitSkeleton.GetBodyPartPosition(hitBodyPartName);
					if (onHit != null) onHit(impactPosition);
				}, null);
	}

	public void PlayPunchSlowAnimation(Vector3 dir, Action<Vector3> onHit,
	                                   Action onAnimComplete,
	                                   float frameRateMod = 1f)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("dBareHands_Punch"), dir, 1f,
				unitAnim2
						=>
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

	public void PlayWebZipShootAnimation(Vector3 dir)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("Spiderman_ShootWebZip"), dir, 1f,
				null,
				null, null);
	}

	public void PlayWebZipFlyingAnimation(Vector3 dir)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("Spiderman_WebZipFlying"), dir, 1f,
				null,
				null, null);
	}

	public void PlaySlidingAnimation(Vector3 dir)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("Spiderman_Sliding"), dir, 1f,
				null, null,
				null);
	}

	public Vector3 GetHandLPosition()
	{
		return unitSkeleton.GetBodyPartPosition("HandL");
	}

	public Vector3 GetHandRPosition()
	{
		return unitSkeleton.GetBodyPartPosition("HandR");
	}

	public void PlayDodgeAnimation(Vector3 dir)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("dBareHands_Roll"), dir, 1f, null,
				null,
				null);
	}

	public void PlayRollAnimation(Vector3 dir)
	{
		unitAnimation.PlayAnimForced(
				UnitAnimType.GetUnitAnimType("dBareHands_Roll"), dir, 1f, null,
				null,
				null);
	}

	public void PlayWinAnimation()
	{
		unitAnimation.PlayAnimForced(UnitAnim.GetUnitAnim("dBareHands_Victory"),
				1f,
				null);
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

	#region BaseSetup
	private V_UnitSkeleton unitSkeleton;
	private V_UnitAnimation unitAnimation;
	private AnimatedWalker animatedWalker;

	private bool lastAttackWasPunch;

	private void Awake()
	{
		var bodyTransform = transform.Find("Body");
		unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint,
				mesh => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
		unitAnimation = new V_UnitAnimation(unitSkeleton);

		var idleUnitAnim = UnitAnimType.GetUnitAnimType("dMarine_Idle");
		var walkUnitAnim = UnitAnimType.GetUnitAnimType("dMarine_Walk");
		var hitUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Hit");
		var attackUnitAnim
				= UnitAnimType.GetUnitAnimType("dBareHands_PunchQuickAttack");

		animatedWalker
				= new AnimatedWalker(unitAnimation, idleUnitAnim, walkUnitAnim,
						1f, 1f);
	}

	private void Update()
	{
		unitSkeleton.Update(Time.deltaTime);
	}

	public void SetIdleWalkAnims(UnitAnimType idleUnitAnim,
	                             UnitAnimType walkUnitAnim)
	{
		animatedWalker
				= new AnimatedWalker(unitAnimation, idleUnitAnim, walkUnitAnim,
						1f, 1f);
	}

	public void RefreshBodySkeletonMesh()
	{
		var bodyTransform = transform.Find("Body");
		bodyTransform.GetComponent<MeshFilter>().mesh = unitSkeleton.GetMesh();
	}

	public V_UnitAnimation GetUnitAnimation()
	{
		return unitAnimation;
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}
	#endregion
}
