#region Info
// -----------------------------------------------------------------------
// CharacterLookAt_Base.cs
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
 * Character Aim Base Class
 * */
public class CharacterLookAt_Base : MonoBehaviour
{


	public void PlayFeetIdleAnim(Vector3 animDir)
	{
		unitSkeletonCompositeWalker_Feet.UpdateBodyParts(false, animDir);
	}

	public void PlayBodyHeadIdleAnim(Vector3 animDir)
	{
		unitSkeletonCompositeWalker_BodyHead.UpdateBodyParts(false, animDir);
	}

	public void PlayFeetWalkAnim(Vector3 animDir)
	{
		unitSkeletonCompositeWalker_Feet.UpdateBodyParts(true, animDir);
	}

	public void PlayBodyHeadWalkAnim(Vector3 animDir)
	{
		unitSkeletonCompositeWalker_BodyHead.UpdateBodyParts(true, animDir);
	}

	public void PlayBodyHeadShootAnim(Vector3 animDir)
	{
		unitSkeleton.ReplaceBodyPartSkeletonAnim(
				GameAssets.UnitAnimTypeEnum.dMarine_Attack.GetUnitAnim(animDir),
				"Body",
				"Head");
	}

	#region BaseSetup
	private V_Object vObject;
	private V_UnitSkeleton unitSkeleton;
	private V_UnitAnimation unitAnimation;
	private V_UnitSkeleton_Composite_Walker
			unitSkeletonCompositeWalker_BodyHead;
	private V_UnitSkeleton_Composite_Walker unitSkeletonCompositeWalker_Feet;
	private V_IObjectTransform objectTransform;

	private void Awake()
	{
		vObject = CreateBasicUnit(transform, GetPosition(), 30f, null);
		unitAnimation = vObject.GetLogic<V_UnitAnimation>();
		unitSkeleton = vObject.GetLogic<V_UnitSkeleton>();
		objectTransform = vObject.GetLogic<V_IObjectTransform>();

		var idleUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Idle");
		var walkUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Walk");
		var hitUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Hit");
		var attackUnitAnim
				= UnitAnimType.GetUnitAnimType("dBareHands_PunchQuickAttack");

		unitSkeletonCompositeWalker_BodyHead
				= new V_UnitSkeleton_Composite_Walker(
						vObject, unitSkeleton,
						GameAssets.UnitAnimTypeEnum.dMarine_Walk,
						GameAssets.UnitAnimTypeEnum.dMarine_Idle,
						new[] { "Body", "Head" });
		vObject.AddRelatedObject(unitSkeletonCompositeWalker_BodyHead);

		unitSkeletonCompositeWalker_Feet = new V_UnitSkeleton_Composite_Walker(
				vObject, unitSkeleton, GameAssets.UnitAnimTypeEnum.dMarine_Walk,
				GameAssets.UnitAnimTypeEnum.dMarine_Idle,
				new[] { "FootL", "FootR" });
		vObject.AddRelatedObject(unitSkeletonCompositeWalker_Feet);
	}

	private static V_Object CreateBasicUnit(Transform unitTransform,
	                                        Vector3 spawnPosition,
	                                        float walkerSpeed,
	                                        Material materialSpriteSheet)
	{
		var unitObject = V_Object.CreateObject(V_Main.UpdateType.Unit);

		var instantiatedTransform = unitTransform;

		V_IObjectTransform transform = new V_ObjectTransform_LateUpdate(
				instantiatedTransform,
				() => instantiatedTransform.transform.position,
				V_Main.RegisterOnLateUpdate, V_Main.DeregisterOnLateUpdate);
		unitObject.AddRelatedObject(transform);
		V_IObjectTransformBody transformBody
				= new V_ObjectTransformBody(instantiatedTransform.Find("Body"),
						materialSpriteSheet);
		unitObject.AddRelatedObject(transformBody);

		var unitSkeleton = new V_UnitSkeleton(1f,
				transformBody.ConvertBodyLocalPositionToWorldPosition,
				transformBody.SetBodyMesh);
		unitObject.AddRelatedObject(unitSkeleton);
		unitObject.AddActiveLogic(
				new V_ObjectLogic_SkeletonUpdater(unitSkeleton));
		var unitAnimation = new V_UnitAnimation(unitSkeleton);
		unitObject.AddRelatedObject(unitAnimation);

		unitObject.AddRelatedObject(
				new V_IHasWorldPosition_Class(transform.GetPosition));

		return unitObject;
	}

	public void SetVObjectEnabled(bool isEnabled)
	{
		vObject.SetIsDisabled(!isEnabled);
	}

	public void RefreshBodySkeletonMesh()
	{
		var bodyTransform = transform.Find("Body");
		bodyTransform.GetComponent<MeshFilter>().mesh = unitSkeleton.GetMesh();
	}

	public void DestroySelf()
	{
		vObject.DestroySelf();
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
