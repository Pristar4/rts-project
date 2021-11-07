#region Info
// -----------------------------------------------------------------------
// V_UnitSkeleton_Composite_Walker.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using V_AnimationSystem;
using V_ObjectSystem;
#endregion

/*
 * Handle setting individual body parts to Walk or Idle based on V_IObjectWalker
 * */
public class V_UnitSkeleton_Composite_Walker
{
	private readonly UnitAnimType idleAnimType;
	private readonly string[] replaceBodyPartArray;

	//private V_IObjectWalker objectWalker;
	private readonly V_UnitSkeleton unitSkeleton;
	private readonly UnitAnimType walkAnimType;

	private V_Object parentObject;

	public V_UnitSkeleton_Composite_Walker(V_Object parentObject,
	                                       V_UnitSkeleton unitSkeleton,
	                                       UnitAnimType walkAnimType,
	                                       UnitAnimType idleAnimType,
	                                       string[] replaceBodyPartArray)
	{
		this.parentObject = parentObject;
		//this.objectWalker = objectWalker;
		this.unitSkeleton = unitSkeleton;
		this.walkAnimType = walkAnimType;
		this.idleAnimType = idleAnimType;
		this.replaceBodyPartArray = replaceBodyPartArray;
	}

	public void UpdateBodyParts(bool isMoving, Vector3 dir)
	{
		//if (objectWalker.IsMoving()) {
		if (isMoving) // Moving
			unitSkeleton.ReplaceBodyPartSkeletonAnim(
					walkAnimType.GetUnitAnim(dir),
					replaceBodyPartArray);
		else // Not moving
			unitSkeleton.ReplaceBodyPartSkeletonAnim(
					idleAnimType.GetUnitAnim(dir),
					replaceBodyPartArray);
	}
}
