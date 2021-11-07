#region Info
// -----------------------------------------------------------------------
// EnemyPathfindingMovement.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using GridPathfindingSystem;
using UnityEngine;
#endregion

/*
 * Responsible for all Enemy Movement Pathfinding
 * */
namespace TopDownShooter
{
	public class EnemyPathfindingMovement : MonoBehaviour
	{

		private const float SPEED = 30f;
		private int currentPathIndex;

		private EnemyMain enemyMain;
		private Vector3 lastMoveDir;
		private Vector3 moveDir;
		private float pathfindingTimer;
		private List<Vector3> pathVectorList;

		private void Awake()
		{
			enemyMain = GetComponent<EnemyMain>();
		}

		private void Update()
		{
			pathfindingTimer -= Time.deltaTime;

			HandleMovement();
		}

		private void FixedUpdate()
		{
			enemyMain.EnemyRigidbody2D.velocity = moveDir * SPEED;
		}

		private void HandleMovement()
		{
			PrintPathfindingPath();
			if (pathVectorList != null)
			{
				var targetPosition = pathVectorList[currentPathIndex];
				var reachedTargetDistance = 5f;
				if (Vector3.Distance(GetPosition(), targetPosition) >
				    reachedTargetDistance)
				{
					moveDir = (targetPosition - GetPosition()).normalized;
					//Debug.Log(moveDir + " " + targetPosition + " " + Vector3.Distance(GetPosition(), targetPosition));
					lastMoveDir = moveDir;
					enemyMain.CharacterAnims.PlayMoveAnim(moveDir);
				}
				else
				{
					currentPathIndex++;
					if (currentPathIndex >= pathVectorList.Count)
					{
						StopMoving();
						enemyMain.CharacterAnims.PlayIdleAnim();
					}
				}
			}
			else { enemyMain.CharacterAnims.PlayIdleAnim(); }
		}

		public void StopMoving()
		{
			pathVectorList = null;
			moveDir = Vector3.zero;
		}

		public List<Vector3> GetPathVectorList()
		{
			return pathVectorList;
		}

		private void PrintPathfindingPath()
		{
			if (pathVectorList != null)
				for (var i = 0; i < pathVectorList.Count - 1; i++)
					Debug.DrawLine(pathVectorList[i], pathVectorList[i + 1]);
		}

		public void MoveToTimer(Vector3 targetPosition)
		{
			if (pathfindingTimer <= 0f) SetTargetPosition(targetPosition);
		}

		public void SetTargetPosition(Vector3 targetPosition)
		{
			currentPathIndex = 0;

			pathVectorList = GridPathfinding.instance
					.GetPathRouteWithShortcuts(GetPosition(), targetPosition)
					.pathVectorList;
			pathfindingTimer = .1f;
			//pathVectorList = new List<Vector3> { targetPosition };

			if (pathVectorList != null && pathVectorList.Count > 1)
				pathVectorList.RemoveAt(0);
		}

		public Vector3 GetPosition()
		{
			return transform.position;
		}

		public Vector3 GetLastMoveDir()
		{
			return lastMoveDir;
		}

		public void Enable()
		{
			enabled = true;
		}

		public void Disable()
		{
			enabled = false;
			enemyMain.EnemyRigidbody2D.velocity = Vector3.zero;
		}
	}

}
