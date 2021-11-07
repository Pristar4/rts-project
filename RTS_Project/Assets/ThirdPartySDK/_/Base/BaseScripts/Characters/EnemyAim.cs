#region Info
// -----------------------------------------------------------------------
// EnemyAim.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion
public class EnemyAim : MonoBehaviour
{


	private const float SPEED = 30f;
	private const float FIRE_RATE = .2f;

	public static List<EnemyAim> enemyList = new List<EnemyAim>();

	private CharacterAim_Base characterBase;
	private int currentPathIndex;

	private Func<IEnemyTargetable> getEnemyTarget;
	private HealthSystem healthSystem;

	private Vector3 lastMoveDir;
	private float nextShootTime;
	private float pathfindingTimer;
	private List<Vector3> pathVectorList;
	private State state;

	private void Awake()
	{
		enemyList.Add(this);
		characterBase = gameObject.GetComponent<CharacterAim_Base>();
		healthSystem = new HealthSystem(1);
		SetStateNormal();

		characterBase.OnShoot += CharacterBase_OnShoot;
	}

	private void Update()
	{
		pathfindingTimer -= Time.deltaTime;

		switch (state)
		{
			case State.Normal:
				HandleMovement();
				FindTarget();
				break;
			case State.Attacking:
				break;
			case State.Busy:
				break;
		}
	}

	public static EnemyAim GetClosestEnemy(Vector3 position, float maxRange)
	{
		EnemyAim closest = null;
		foreach (var enemy in enemyList)
		{
			if (enemy.IsDead()) continue;
			if (Vector3.Distance(position, enemy.GetPosition()) <= maxRange)
			{
				if (closest == null) { closest = enemy; }
				else
				{
					if (Vector3.Distance(position, enemy.GetPosition()) <
					    Vector3.Distance(position, closest.GetPosition()))
						closest = enemy;
				}
			}
		}
		return closest;
	}

	public static EnemyAim Create(Vector3 position)
	{
		var enemyTransform
				= Instantiate(GameAssets.i.pfEnemy, position,
						Quaternion.identity);

		var enemyHandler = enemyTransform.GetComponent<EnemyAim>();

		return enemyHandler;
	}

	private void CharacterBase_OnShoot(object sender,
	                                   CharacterAim_Base.OnShootEventArgs e)
	{
		Shoot_Flash.AddFlash(e.gunEndPointPosition);
		WeaponTracer.Create(e.gunEndPointPosition, e.shootPosition);
		UtilsClass.ShakeCamera(.3f, .05f);

		// Any hit? Player?
		var raycastHit = Physics2D.Raycast(e.gunEndPointPosition,
				(e.shootPosition - e.gunEndPointPosition).normalized,
				Vector3.Distance(e.gunEndPointPosition, e.shootPosition));
		if (raycastHit.collider != null)
		{
			var player
					= raycastHit.collider.gameObject.GetComponent<PlayerAim>();
			if (player != null) player.Damage(this);
		}
	}

	public void SetGetTarget(Func<IEnemyTargetable> getEnemyTarget)
	{
		this.getEnemyTarget = getEnemyTarget;
	}

	private void SetStateNormal()
	{
		state = State.Normal;
	}

	private void FindTarget()
	{
		var targetRange = 160f;
		var attackRange = 50f;
		if (getEnemyTarget != null)
		{
			var targetPosition = getEnemyTarget().GetPosition();
			if (Vector3.Distance(getEnemyTarget().GetPosition(),
					    GetPosition()) <
			    attackRange)
			{
				StopMoving();
				characterBase.SetAimTarget(targetPosition);

				if (Time.time >= nextShootTime)
				{
					// Can shoot
					nextShootTime = Time.time + FIRE_RATE;
					targetPosition += UtilsClass.GetRandomDir() *
					                  Random.Range(0f, 20f);
					characterBase.ShootTarget(targetPosition);
				}
				/*
				state = State.Attacking;
				Vector3 attackDir = (getEnemyTarget().GetPosition() - GetPosition()).normalized;
				characterBase.ShootTarget(getEnemyTarget().GetPosition(), () => {
				    if (getEnemyTarget() != null) {
				        getEnemyTarget().Damage(this);
				    }
				}, SetStateNormal);
				*/
			}
			else
			{
				if (Vector3.Distance(getEnemyTarget().GetPosition(),
						    GetPosition()) <
				    targetRange)
				{
					if (pathfindingTimer <= 0f)
					{
						pathfindingTimer = .3f;
						SetTargetPosition(getEnemyTarget().GetPosition());
					}
					characterBase.SetAimTarget(targetPosition);
				}
			}
		}
	}

	public bool IsDead()
	{
		return healthSystem.IsDead();
	}

	private void SetStateAttacking()
	{
		state = State.Attacking;
	}

	public void Damage(IEnemyTargetable attacker)
	{
		var bloodDir = (GetPosition() - attacker.GetPosition()).normalized;
		Blood_Handler.SpawnBlood(GetPosition(), bloodDir);

		healthSystem.Damage(30);
		if (IsDead())
		{
			FlyingBody.Create(GameAssets.i.pfEnemyFlyingBody, GetPosition(),
					bloodDir);
			characterBase.DestroySelf();
			Destroy(gameObject);
		}
		else
		{
			// Knockback
			transform.position += bloodDir * 5f;
			/*
			if (hitUnitAnim != null) {
			    state = State.Busy;
			    enemyBase.PlayHitAnimation(bloodDir * (Vector2.one * -1f), SetStateNormal);
			}
			*/
		}
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
				characterBase.PlayMoveAnim(moveDir);
				transform.position
						= transform.position + moveDir * SPEED * Time.deltaTime;
			}
			else
			{
				currentPathIndex++;
				if (currentPathIndex >= pathVectorList.Count)
				{
					StopMoving();
					characterBase.PlayIdleAnim();
				}
			}
		}
		else { characterBase.PlayIdleAnim(); }
	}

	private void StopMoving()
	{
		pathVectorList = null;
	}

	public void SetTargetPosition(Vector3 targetPosition)
	{
		currentPathIndex = 0;
		//pathVectorList = GridPathfinding.instance.GetPathRouteWithShortcuts(GetPosition(), targetPosition).pathVectorList;
		pathVectorList = new List<Vector3> { targetPosition };
		if (pathVectorList != null && pathVectorList.Count > 1)
			pathVectorList.RemoveAt(0);
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public interface IEnemyTargetable
	{
		Vector3 GetPosition();
		void Damage(EnemyAim attacker);
	}


	private enum State
	{
		Normal,
		Attacking,
		Busy
	}
}
