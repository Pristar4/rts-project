#region Info
// -----------------------------------------------------------------------
// CharacterPathfindingMovementHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
using V_AnimationSystem;
#endregion
public class CharacterPathfindingMovementHandler : MonoBehaviour
{

	private const float speed = 40f;
	private AnimatedWalker animatedWalker;
	private int currentPathIndex;
	private List<Vector3> pathVectorList;
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
		if (pathVectorList != null)
		{
			var targetPosition = pathVectorList[currentPathIndex];
			if (Vector3.Distance(transform.position, targetPosition) > 1f)
			{
				var moveDir = (targetPosition - transform.position).normalized;

				var distanceBefore
						= Vector3.Distance(transform.position, targetPosition);
				animatedWalker.SetMoveVector(moveDir);
				transform.position
						= transform.position + moveDir * speed * Time.deltaTime;
			}
			else
			{
				currentPathIndex++;
				if (currentPathIndex >= pathVectorList.Count)
				{
					StopMoving();
					animatedWalker.SetMoveVector(Vector3.zero);
				}
			}
		}
		else { animatedWalker.SetMoveVector(Vector3.zero); }
	}

	private void StopMoving()
	{
		pathVectorList = null;
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public void SetTargetPosition(Vector3 targetPosition)
	{
		currentPathIndex = 0;
		//pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition);

		if (pathVectorList != null && pathVectorList.Count > 1)
			pathVectorList.RemoveAt(0);
	}
}
