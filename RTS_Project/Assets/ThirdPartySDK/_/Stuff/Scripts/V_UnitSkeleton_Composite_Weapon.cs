#region Info
// -----------------------------------------------------------------------
// V_UnitSkeleton_Composite_Weapon.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
using V_AnimationSystem;
using V_ObjectSystem;
#endregion

/*
 * Manages the Composite Skeleton to Aim and Shoot a Weapon
 * Doesnt manage Feet body parts
 * */
public class V_UnitSkeleton_Composite_Weapon : V_IActiveInactive
{

	private readonly V_Object parentObject;

	private readonly V_UnitSkeleton unitSkeleton;
	private UnitAnim activeAnimAimWeapon;
	private UnitAnim activeAnimShootWeapon;
	private Vector3 aimTargetPosition;
	private UnitAnim animAimWeaponLeft;
	private UnitAnim animAimWeaponRight;
	private UnitAnim animShootWeaponLeft;
	private UnitAnim animShootWeaponRight;
	private bool isShooting;
	private Vector3 positionOffset;
	private bool
			usingSkeletonRight; // Currently using Normal or inverted V anim

	//private string debugText = "";

	public V_UnitSkeleton_Composite_Weapon(V_Object parentObject,
	                                       V_UnitSkeleton unitSkeleton,
	                                       UnitAnim animAimWeaponRight,
	                                       UnitAnim animAimWeaponLeft,
	                                       UnitAnim animShootWeaponRight,
	                                       UnitAnim animShootWeaponLeft)
	{
		this.parentObject = parentObject;
		this.unitSkeleton = unitSkeleton;
		this.animAimWeaponRight = animAimWeaponRight.CloneDeep();
		this.animAimWeaponLeft = animAimWeaponLeft.CloneDeep();
		this.animShootWeaponRight = animShootWeaponRight.CloneDeep();
		this.animShootWeaponLeft = animShootWeaponLeft.CloneDeep();

		SetPositionOffset(new Vector3(0, -2));
		SetInactive();

		//CodeMonkey.CMDebug.TextUpdater(() => debugText, Vector3.zero, parentObject.GetLogic<V_IObjectTransform>().GetTransform());
	}

	public void SetActive()
	{
		activeAnimAimWeapon = animAimWeaponRight;
		activeAnimShootWeapon = animShootWeaponRight;
		unitSkeleton.ReplaceAllBodyPartsInAnimation(activeAnimAimWeapon);
		usingSkeletonRight = true;
		unitSkeleton.GetSkeletonUpdater().SetHasVariableSortingOrder(true);
		isShooting = false;
	}

	public void SetInactive() { }

	public void SetPositionOffset(Vector3 positionOffset)
	{
		this.positionOffset = positionOffset;
	}

	public void SetAimTarget(Vector3 aimTargetPosition)
	{
		this.aimTargetPosition = aimTargetPosition;

		var aimDir = (aimTargetPosition - parentObject.GetPosition())
				.normalized;

		//debugText = ""+usingSkeletonRight + " " + aimDir;

		// Decide if should use Right or Left Body Part
		if (!isShooting)
			switch (UnitAnim.GetAnimDirFromVector(aimDir))
			{
				default:
				case UnitAnim.AnimDir.Down:
				case UnitAnim.AnimDir.DownRight:
				case UnitAnim.AnimDir.Right:
				case UnitAnim.AnimDir.UpRight:
				case UnitAnim.AnimDir.Up:
					if (!usingSkeletonRight)
					{
						// Switch sides
						usingSkeletonRight = true;
						activeAnimAimWeapon = animAimWeaponRight;
						activeAnimShootWeapon = animShootWeaponRight;
						unitSkeleton.ReplaceAllBodyPartsInAnimation(
								activeAnimAimWeapon);
					}
					break;
				case UnitAnim.AnimDir.UpLeft:
				case UnitAnim.AnimDir.Left:
				case UnitAnim.AnimDir.DownLeft:
					if (usingSkeletonRight)
					{
						// Switch sides
						//CodeMonkey.CMDebug.TextPopup("ChangeLeft", parentObject.GetPosition());
						usingSkeletonRight = false;
						activeAnimAimWeapon = animAimWeaponLeft;
						activeAnimShootWeapon = animShootWeaponLeft;
						unitSkeleton.ReplaceAllBodyPartsInAnimation(
								activeAnimAimWeapon);
					}
					break;
			}

		// Show on top of Body for all except Up
		var weaponOnTopOfBody
				= UnitAnim.GetAnimDirFromVectorLimit4Directions(aimDir) !=
				  UnitAnim.AnimDir.Up;

		var bonusOffset = 2000;

		if (usingSkeletonRight)
		{
			activeAnimAimWeapon.ApplyAimDir(aimDir, positionOffset,
					weaponOnTopOfBody ? +bonusOffset : -bonusOffset);
			activeAnimShootWeapon.ApplyAimDir(aimDir, positionOffset,
					weaponOnTopOfBody ? +bonusOffset : -bonusOffset);
		}
		else
		{
			activeAnimAimWeapon.ApplyAimDir(
					UtilsClass.ApplyRotationToVector(aimDir, 180),
					positionOffset,
					weaponOnTopOfBody ? +bonusOffset : -bonusOffset);
			activeAnimShootWeapon.ApplyAimDir(
					UtilsClass.ApplyRotationToVector(aimDir, 180),
					positionOffset,
					weaponOnTopOfBody ? +bonusOffset : -bonusOffset);
		}
	}

	public void Shoot(Vector3 shootTargetPosition, Action onShootCompleted)
	{
		SetAimTarget(shootTargetPosition);

		Action<V_Skeleton_Anim> shootCompleted = skeletonAnim =>
		{
			activeAnimShootWeapon.GetAnims()[0].onAnimComplete = null;
			isShooting = false;
			unitSkeleton.ReplaceAllBodyPartsInAnimation(activeAnimAimWeapon);
			onShootCompleted();
		};
		activeAnimShootWeapon.ResetAnims();
		activeAnimShootWeapon.GetAnims()[0].onAnimComplete = shootCompleted;
		unitSkeleton.ReplaceAllBodyPartsInAnimation(activeAnimShootWeapon);
		isShooting = true;
	}

	public void SetAnims(UnitAnim animAimWeaponRight,
	                     UnitAnim animAimWeaponLeft,
	                     UnitAnim animShootWeaponRight,
	                     UnitAnim animShootWeaponLeft)
	{
		this.animAimWeaponRight = animAimWeaponRight.CloneDeep();
		this.animAimWeaponLeft = animAimWeaponLeft.CloneDeep();
		this.animShootWeaponRight = animShootWeaponRight.CloneDeep();
		this.animShootWeaponLeft = animShootWeaponLeft.CloneDeep();
		SetActive();
	}
}
