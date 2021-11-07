#region Info
// -----------------------------------------------------------------------
// EnemyAimLogic.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

/*
 * Enemy Minion with Aim
 * Handles Find Target, Move to it, Aim and Shoot
 * */
namespace TopDownShooter
{
	public class EnemyAimLogic : MonoBehaviour
	{

		private const float FIRE_RATE = .2f;
		private const float START_AIM_ANGLE = 20f;
		private IAimShootAnims aimAnims;
		private AimingShoot aimingShoot;
		private CharacterAim_Base characterBase;

		private EnemyMain enemyMain;
		private Enemy.IEnemyTargetable enemyTarget;
		private float nextShootTime;
		private State state;

		private void Awake()
		{
			enemyMain = GetComponent<EnemyMain>();
			characterBase = GetComponent<CharacterAim_Base>();
			aimAnims = GetComponent<IAimShootAnims>();
			aimingShoot = GetComponent<AimingShoot>();

			SetStateNormal();
		}

		private void Start()
		{
			characterBase.SetWeaponType(CharacterAim_Base.WeaponType.Pistol);
			enemyMain.OnDestroySelf += EnemyMain_OnDestroySelf;
			enemyMain.HealthSystem.SetHealthMax(100, true);
		}

		private void Update()
		{
			switch (state)
			{
				case State.Normal:
					enemyTarget = enemyMain.EnemyTargeting.GetActiveTarget();
					if (enemyTarget != null)
					{
						state = State.AimingAtTarget;
						aimingShoot.StartAimingAtTarget(
								enemyTarget.GetPosition(),
								START_AIM_ANGLE, 0f, 10f, ShootTarget);
						/*
						float attackRange = 50f;
						Vector3 targetPosition = enemyTarget.GetPosition();
						if (Vector3.Distance(GetPosition(), targetPosition) < attackRange) {
						    // Shoot Target
						    enemyMain.EnemyMovement.StopMoving();
						    characterBase.SetAimTarget(targetPosition);
			  
						    if (Time.time >= nextShootTime) {
						        // Can shoot
						        nextShootTime = Time.time + FIRE_RATE;
						        targetPosition += UtilsClass.GetRandomDir() * UnityEngine.Random.Range(0f, 20f);
						        SetStateAttacking();
						        characterBase.ShootTarget(targetPosition, SetStateNormal);
						    }
						} else {
						    // Move to Target
						    enemyMain.EnemyMovement.SetTargetPosition(targetPosition);
						    characterBase.SetAimTarget(targetPosition);
						}
						*/
					}
					break;
				case State.AimingAtTarget:
					enemyTarget = enemyMain.EnemyTargeting.GetActiveTarget();
					if (enemyTarget != null)
					{
						var targetPosition = enemyTarget.GetPosition();
						aimAnims.SetAimTarget(targetPosition);
						aimingShoot.UpdateAimShootAtTarget(targetPosition);
						/*
						aimingShoot.AimAtPosition(targetPosition);
						aimAnims.SetAimTarget(targetPosition);
			  
						float aimSpeed = 10f;
						aimingShoot.SetAimAngle(aimAngle - aimSpeed * Time.deltaTime);
			  
						aimingShoot.SetAimColor(Mathf.Lerp(.0f, 1.1f, 1 - aimAngle / START_AIM_ANGLE));
			  
						float shootAngle = 0f;
						if (aimAngle <= shootAngle) {
						    // Shoot!
						    SetStateAttacking();
						    aimAnims.ShootTarget(targetPosition, SetStateNormal);
						    aimingShoot.HideAim();
						}
						*/
					}
					break;
				case State.Attacking:
					break;
				case State.Busy:
					break;
			}
		}

		private void EnemyMain_OnDestroySelf(object sender, EventArgs e)
		{
			characterBase.DestroySelf();
		}

		private void ShootTarget()
		{
			enemyTarget = enemyMain.EnemyTargeting.GetActiveTarget();
			if (enemyTarget != null)
			{
				// Shoot!
				var targetPosition = enemyTarget.GetPosition();

				SetStateAttacking();
				aimAnims.ShootTarget(targetPosition, SetStateNormal);
			}
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
			AimingAtTarget,
			Attacking,
			Busy
		}
	}

}
