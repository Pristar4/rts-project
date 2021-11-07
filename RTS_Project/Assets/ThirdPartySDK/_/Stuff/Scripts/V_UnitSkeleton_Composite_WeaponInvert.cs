#region Info
// -----------------------------------------------------------------------
// V_UnitSkeleton_Composite_WeaponInvert.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
using V_AnimationSystem;
using V_ObjectSystem;
#endregion

/*
 * Manages the Composite Skeleton to Aim and Shoot a Weapon
 * Doesnt manage Feet body parts
 * */
public class V_UnitSkeleton_Composite_WeaponInvert : V_IActiveInactive
{
	private readonly UnitAnim animAimWeaponRight;
	private readonly UnitAnim animAimWeaponRightInvertV;
	private readonly UnitAnim animShootWeaponRight;
	private readonly UnitAnim animShootWeaponRightInvertV;

	private readonly V_Object parentObject;

	private readonly V_UnitSkeleton unitSkeleton;
	private UnitAnim activeAnimAimWeaponRight;
	private UnitAnim activeAnimShootWeaponRight;
	private Vector3 aimTargetPosition;
	private bool isShooting;
	private bool
			usingSkeletonNormal; // Currently using Normal or inverted V anim

	public V_UnitSkeleton_Composite_WeaponInvert(V_Object parentObject,
	                                             V_UnitSkeleton unitSkeleton,
	                                             UnitAnim animAimWeaponRight,
	                                             UnitAnim
			                                             animAimWeaponRightInvertV,
	                                             UnitAnim animShootWeaponRight,
	                                             UnitAnim
			                                             animShootWeaponRightInvertV)
	{
		this.parentObject = parentObject;
		this.unitSkeleton = unitSkeleton;
		this.animAimWeaponRight = animAimWeaponRight.Clone();
		this.animAimWeaponRightInvertV = animAimWeaponRightInvertV.Clone();
		this.animShootWeaponRight = animShootWeaponRight.Clone();
		this.animShootWeaponRightInvertV = animShootWeaponRightInvertV.Clone();

		SetInactive();
	}

	public void SetActive()
	{
		activeAnimAimWeaponRight = animAimWeaponRight;
		activeAnimShootWeaponRight = animShootWeaponRight;
		unitSkeleton.ReplaceAllBodyPartsInAnimation(activeAnimAimWeaponRight);
		usingSkeletonNormal = true;
		unitSkeleton.GetSkeletonUpdater().SetHasVariableSortingOrder(true);
	}

	public void SetInactive() { }

	public void SetAimTarget(Vector3 aimTargetPosition)
	{
		this.aimTargetPosition = aimTargetPosition;

		var aimDir = (aimTargetPosition - parentObject.GetPosition())
				.normalized;

		// Decide if should use Inverted Vertical Body Part
		if (!isShooting)
			switch (UnitAnim.GetAnimDirFromVector(aimDir))
			{
				default:
				case UnitAnim.AnimDir.Down:
				case UnitAnim.AnimDir.DownRight:
				case UnitAnim.AnimDir.Right:
				case UnitAnim.AnimDir.UpRight:
				case UnitAnim.AnimDir.Up:
					if (!usingSkeletonNormal)
					{
						// Switch sides
						usingSkeletonNormal = true;
						activeAnimAimWeaponRight = animAimWeaponRight;
						activeAnimShootWeaponRight = animShootWeaponRight;
						unitSkeleton.ReplaceAllBodyPartsInAnimation(
								activeAnimAimWeaponRight);
					}
					break;
				case UnitAnim.AnimDir.UpLeft:
				case UnitAnim.AnimDir.Left:
				case UnitAnim.AnimDir.DownLeft:
					if (usingSkeletonNormal)
					{
						// Switch sides
						usingSkeletonNormal = false;
						activeAnimAimWeaponRight = animAimWeaponRightInvertV;
						activeAnimShootWeaponRight
								= animShootWeaponRightInvertV;
						unitSkeleton.ReplaceAllBodyPartsInAnimation(
								activeAnimAimWeaponRight);
					}
					break;
			}

		// Show on top of Body for all except Up
		var weaponOnTopOfBody
				= UnitAnim.GetAnimDirFromVectorLimit4Directions(aimDir) !=
				  UnitAnim.AnimDir.Up;

		activeAnimAimWeaponRight.ApplyAimDir(aimDir, new Vector3(0, -2),
				weaponOnTopOfBody ? +1000 : -1000);
		activeAnimShootWeaponRight.ApplyAimDir(aimDir, new Vector3(0, -2),
				weaponOnTopOfBody ? +1000 : -1000);
	}

	public void Shoot(Vector3 shootTargetPosition, Action onShootCompleted)
	{
		SetAimTarget(shootTargetPosition);

		Action<V_Skeleton_Anim> shootCompleted = skeletonAnim =>
		{
			isShooting = false;
			unitSkeleton.ReplaceAllBodyPartsInAnimation(
					activeAnimAimWeaponRight);
			onShootCompleted();
		};
		activeAnimShootWeaponRight.ResetAnims();
		activeAnimShootWeaponRight.GetAnims()[0].onAnimComplete
				= shootCompleted;
		unitSkeleton.ReplaceAllBodyPartsInAnimation(activeAnimShootWeaponRight);
		isShooting = true;
	}
}
