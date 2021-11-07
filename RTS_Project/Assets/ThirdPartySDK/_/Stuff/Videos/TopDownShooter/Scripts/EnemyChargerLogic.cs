#region Info
// -----------------------------------------------------------------------
// EnemyChargerLogic.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion

/*
 * Enemy Charger
 * */
namespace TopDownShooter
{
	public class EnemyChargerLogic : MonoBehaviour
	{
		private Transform aimTransform;
		private Character_Base characterBase;
		private float chargeDelay;
		private Vector3 chargeDir;
		private float chargeSpeed;

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
			enemyMain.HealthSystem.SetHealthMax(120, true);
		}

		private void Update()
		{
			switch (state)
			{
				case State.Normal:
					chargeDelay -= Time.deltaTime;
					enemyTarget = enemyMain.EnemyTargeting.GetActiveTarget();
					if (enemyTarget != null)
					{
						var targetPosition = enemyTarget.GetPosition();
						if (chargeDelay > 0)
						{
							// Too soon to charge, move to it
							enemyMain.EnemyPathfindingMovement.MoveToTimer(
									targetPosition);
							//Debug.Log(enemyMain.EnemyPathfindingMovement);
						}
						else
						{
							if (CanChargeToTarget(targetPosition,
									enemyTarget.GetGameObject()))
							{
								chargeDir = (targetPosition - GetPosition())
										.normalized;
								SetAimTarget(targetPosition);
								ShowAim();
								chargeSpeed = 200f;
								enemyMain.EnemyPathfindingMovement.Disable();
								characterBase.PlayDodgeAnimation(chargeDir);
								state = State.Charging;
							}
							else
							{
								// Cannot see target, move to it
								enemyMain.EnemyPathfindingMovement.MoveToTimer(
										targetPosition);
							}
						}
					}
					break;
				case State.Charging:
					var chargeSpeedDropMultiplier = 1f;
					chargeSpeed -= chargeSpeed * chargeSpeedDropMultiplier *
					               Time.deltaTime;

					var hitDistance = 3f;
					var hitLayerMask = ~0;
					var raycastHit2D = Physics2D.Raycast(GetPosition(),
							chargeDir,
							hitDistance,
							hitLayerMask); // 1 << GameAssets.i.playerLayer);
					if (raycastHit2D.collider != null)
					{
						var player
								= raycastHit2D.collider.GetComponent<Player>();
						if (player != null)
						{
							player.Damage(enemyMain.Enemy, .6f);
							player.Knockback(chargeDir, 5f);
							chargeSpeed = 60f;
							chargeDir *= -1f;
						}
					}

					var chargeSpeedMinimum = 70f;
					if (chargeSpeed < chargeSpeedMinimum)
					{
						state = State.Normal;
						enemyMain.EnemyPathfindingMovement.Enable();
						chargeDelay = 1.5f;
						SetStateNormal();
						HideAim();
					}
					break;
				case State.Attacking:
					break;
				case State.Busy:
					break;
			}
		}

		private void FixedUpdate()
		{
			switch (state)
			{
				case State.Charging:
					enemyMain.EnemyRigidbody2D.velocity
							= chargeDir * chargeSpeed;
					break;
			}
		}

		private bool CanChargeToTarget(Vector3 targetPosition,
		                               GameObject targetGameObject)
		{
			var targetDistance
					= Vector3.Distance(GetPosition(), targetPosition);

			var maxChargeDistance = 70f;
			if (targetDistance > maxChargeDistance) return false;

			var dirToTarget = (targetPosition - GetPosition()).normalized;
			var hitLayerMask = ~0;
			var raycastHit2D = Physics2D.Raycast(GetPosition(), dirToTarget,
					targetDistance,
					hitLayerMask); // ~(1 << GameAssets.i.enemyLayer | 1 << GameAssets.i.ignoreRaycastLayer));
			return raycastHit2D.collider == null ||
			       raycastHit2D.collider.gameObject == targetGameObject;
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
			Charging,
			Attacking,
			Busy
		}
	}

}
