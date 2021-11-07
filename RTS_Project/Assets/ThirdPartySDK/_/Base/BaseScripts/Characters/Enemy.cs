#region Info
// -----------------------------------------------------------------------
// Enemy.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using UnityEngine;
using V_AnimationSystem;
#endregion

/*
 * Enemy
 * */
public class Enemy : MonoBehaviour
{


	private const float SPEED = 30f;

	public static List<Enemy> enemyList = new List<Enemy>();
	private UnitAnimType attackUnitAnim;
	private Character_Base characterBase;
	private int currentPathIndex;
	private Func<IEnemyTargetable> getEnemyTarget;

	private HealthSystem healthSystem;
	private UnitAnimType hitUnitAnim;

	private UnitAnimType idleUnitAnim;
	private Vector3 lastMoveDir;
	private float pathfindingTimer;
	private List<Vector3> pathVectorList;
	private State state;
	private UnitAnimType walkUnitAnim;

	private void Awake()
	{
		enemyList.Add(this);
		characterBase = gameObject.GetComponent<Character_Base>();
		healthSystem = new HealthSystem(100);
		SetStateNormal();
	}

	private void Start()
	{
		/*
		World_Bar healthBar = new World_Bar(transform, new Vector3(0, 9), new Vector3(7, 1.5f), Color.grey, Color.red, 1f, 1000, new World_Bar.Outline { color = Color.black, size = .5f });
		healthSystem.OnHealthChanged += (object sender, EventArgs e) => {
		    healthBar.SetSize(healthSystem.GetHealthNormalized());
		};
		*/
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

	public static Enemy GetClosestEnemy(Vector3 position, float maxRange)
	{
		Enemy closest = null;
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


	public static Enemy Create(Vector3 position)
	{
		var enemyTransform
				= Instantiate(GameAssets.i.pfEnemy, position,
						Quaternion.identity);

		var enemyHandler = enemyTransform.GetComponent<Enemy>();

		return enemyHandler;
	}

	public void SetGetTarget(Func<IEnemyTargetable> getEnemyTarget)
	{
		this.getEnemyTarget = getEnemyTarget;
	}

	private void FindTarget()
	{
		var targetRange = 100f;
		var attackRange = 15f;
		if (getEnemyTarget != null)
		{
			if (Vector3.Distance(getEnemyTarget().GetPosition(),
					    GetPosition()) <
			    attackRange)
			{
				StopMoving();
				state = State.Attacking;
				var attackDir = (getEnemyTarget().GetPosition() - GetPosition())
						.normalized;
				characterBase.PlayPunchSlowAnimation(attackDir, hitPosition =>
				{
					if (getEnemyTarget() != null) getEnemyTarget().Damage(this);
				}, SetStateNormal);
			}
			else
			{
				if (Vector3.Distance(getEnemyTarget().GetPosition(),
						    GetPosition()) <
				    targetRange)
					if (pathfindingTimer <= 0f)
					{
						pathfindingTimer = .3f;
						SetTargetPosition(getEnemyTarget().GetPosition());
					}
			}
		}
	}

	public bool IsDead()
	{
		return healthSystem.IsDead();
	}

	private void SetStateNormal()
	{
		state = State.Normal;
	}

	private void SetStateAttacking()
	{
		state = State.Attacking;
	}

	public void Damage(IEnemyTargetable attacker)
	{
		var bloodDir = (GetPosition() - attacker.GetPosition()).normalized;
		Blood_Handler.SpawnBlood(GetPosition(), bloodDir);

		var damageAmount = 30;
		DamagePopup.Create(GetPosition(), damageAmount, false);
		healthSystem.Damage(damageAmount);
		if (IsDead())
		{
			FlyingBody.Create(GameAssets.i.pfEnemyFlyingBody, GetPosition(),
					bloodDir);
			Destroy(gameObject);
		}
		else
		{
			// Knockback
			transform.position += bloodDir * 5f;
			if (hitUnitAnim != null)
			{
				state = State.Busy;
				characterBase.PlayHitAnimation(bloodDir * (Vector2.one * -1f),
						SetStateNormal);
			}
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
		void Damage(Enemy attacker);
	}

	private enum State
	{
		Normal,
		Attacking,
		Busy
	}
}
