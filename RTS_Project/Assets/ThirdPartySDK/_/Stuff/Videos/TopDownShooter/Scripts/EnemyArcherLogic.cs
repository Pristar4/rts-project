#region Info
// -----------------------------------------------------------------------
// EnemyArcherLogic.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion

/*
 * Enemy Archer, throw Shuriken
 * */
namespace TopDownShooter
{
	public class EnemyArcherLogic : MonoBehaviour
	{
		private Transform aimTransform;
		private Character_Base characterBase;

		private EnemyMain enemyMain;
		private Enemy.IEnemyTargetable enemyTarget;
		private State state;

		private void Awake()
		{
			enemyMain = GetComponent<EnemyMain>();
			characterBase = GetComponent<Character_Base>();

			aimTransform = transform.Find("Aim");

			SetStateNormal();
			HideAim();
		}

		private void Start()
		{
			enemyMain.HealthSystem.SetHealthMax(50, true);
		}

		private void Update()
		{
			switch (state)
			{
				case State.Normal:
					enemyTarget = enemyMain.EnemyTargeting.GetActiveTarget();
					if (enemyTarget != null)
					{
						var targetPosition = enemyTarget.GetPosition();
						enemyMain.EnemyPathfindingMovement.MoveToTimer(
								targetPosition);
						var attackDistance = 80f;
						var targetDistance
								= Vector3.Distance(GetPosition(),
										targetPosition);
						if (targetDistance < attackDistance)
						{
							// Target within attack distance
							//int layerMask = ~(1 << GameAssets.i.enemyLayer | 1 << GameAssets.i.ignoreRaycastLayer | 1 << GameAssets.i.shieldLayer);
							var layerMask = ~0;
							var raycastHit2D = Physics2D.Raycast(GetPosition(),
									(targetPosition - GetPosition()).normalized,
									targetDistance,
									layerMask);
							if (raycastHit2D.collider != null &&
							    raycastHit2D.collider.GetComponent<Player>())
							{
								// Player in line of sight
								enemyMain.EnemyPathfindingMovement.Disable();
								SetStateAttacking();
								var targetDir = (targetPosition - GetPosition())
										.normalized;
								characterBase.PlayPunchAnimation(targetDir,
										hitPosition =>
										{
											// Throw rock
											enemyTarget = enemyMain
													.EnemyTargeting
													.GetActiveTarget();
											if (enemyTarget != null)
											{
												var throwDir
														= (enemyTarget
																		.GetPosition() -
																hitPosition)
														.normalized;
												var enemyShuriken
														= EnemyShuriken.Create(
																enemyMain.Enemy,
																hitPosition,
																throwDir);
											}
											//CMDebug.TextPopup("#", hitPosition);
										}, () =>
										{
											// Punch complete
											enemyMain.EnemyPathfindingMovement
													.Enable();
											SetStateNormal();
										});
							}
						}
					}
					break;
				case State.Attacking:
					break;
				case State.Busy:
					break;
			}
		}

		public void SetAimTarget(Vector3 targetPosition)
		{
			var aimDir = (targetPosition - transform.position).normalized;
			aimTransform.eulerAngles
					= new Vector3(0, 0,
							UtilsClass.GetAngleFromVectorFloat(aimDir));
		}

		private void ShowAim()
		{
			aimTransform.gameObject.SetActive(true);
		}

		private void HideAim()
		{
			aimTransform.gameObject.SetActive(false);
		}

		private void SetStateNormal()
		{
			state = State.Normal;
		}

		private void SetStateAttacking()
		{
			state = State.Attacking;
		}

		public Vector3 GetPosition()
		{
			return transform.position;
		}

		private enum State
		{
			Normal,
			Attacking,
			Busy
		}
	}
}
