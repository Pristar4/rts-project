#region Info
// -----------------------------------------------------------------------
// CharacterPositionMovementHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
using V_AnimationSystem;
#endregion
public class CharacterPositionMovementHandler : MonoBehaviour
{

	private const float speed = 40f;
	private AnimatedWalker animatedWalker;
	private Vector3 targetPosition;
	private V_UnitAnimation unitAnimation;

	private V_UnitSkeleton unitSkeleton;


	private void Start()
	{
		var bodyTransform = transform.Find("Body");
		unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint,
				mesh => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
		unitAnimation = new V_UnitAnimation(unitSkeleton);
		animatedWalker = new AnimatedWalker(unitAnimation,
				UnitAnimType.GetUnitAnimType("dMarine_Idle"),
				UnitAnimType.GetUnitAnimType("dMarine_Walk"), 1f, 1f);
	}

	private void Update()
	{
		HandleMovement();
		unitSkeleton.Update(Time.deltaTime);

		if (Input.GetMouseButtonDown(0))
			SetTargetPosition(UtilsClass.GetMouseWorldPosition());
	}

	private void HandleMovement()
	{
		if (Vector3.Distance(transform.position, targetPosition) > 1f)
		{
			var moveDir = (targetPosition - transform.position).normalized;

			var distanceBefore
					= Vector3.Distance(transform.position, targetPosition);
			animatedWalker.SetMoveVector(moveDir);
			transform.position
					= transform.position + moveDir * speed * Time.deltaTime;
		}
		else { animatedWalker.SetMoveVector(Vector3.zero); }
	}


	public void SetTargetPosition(Vector3 targetPosition)
	{
		targetPosition.z = 0f;
		this.targetPosition = targetPosition;
	}
}
