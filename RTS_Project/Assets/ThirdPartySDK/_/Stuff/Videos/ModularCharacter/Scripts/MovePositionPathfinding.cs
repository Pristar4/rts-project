#region Info
// -----------------------------------------------------------------------
// MovePositionPathfinding.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using GridPathfindingSystem;
using UnityEngine;
#endregion
public class MovePositionPathfinding : MonoBehaviour, IMovePosition
{
	private int pathIndex = -1;

	private List<Vector3> pathVectorList;

	private void Update()
	{
		if (pathIndex != -1)
		{
			// Move to next path position
			var nextPathPosition = pathVectorList[pathIndex];
			var moveVelocity
					= (nextPathPosition - transform.position).normalized;
			GetComponent<IMoveVelocity>().SetVelocity(moveVelocity);

			var reachedPathPositionDistance = 1f;
			if (Vector3.Distance(transform.position, nextPathPosition) <
			    reachedPathPositionDistance)
			{
				pathIndex++;
				if (pathIndex >= pathVectorList.Count) // End of path
					pathIndex = -1;
			}
		}
		else
		{
			// Idle
			GetComponent<IMoveVelocity>().SetVelocity(Vector3.zero);
		}
	}

	public void SetMovePosition(Vector3 movePosition)
	{
		pathVectorList = GridPathfinding.instance
				.GetPathRouteWithShortcuts(transform.position, movePosition)
				.pathVectorList;
		if (pathVectorList.Count >
		    0) // Remove first position so he doesn't go backwards
			pathVectorList.RemoveAt(0);
		if (pathVectorList.Count > 0)
			pathIndex = 0;
		else
			pathIndex = -1;
	}
}
