#region Info
// -----------------------------------------------------------------------
// EnemyHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using GridPathfindingSystem;
using UnityEngine;
using V_AnimationSystem;
#endregion
public class EnemyHandler : MonoBehaviour
{


	private const float speed = 30f;

	public static List<EnemyHandler>
			enemyHandlerList = new List<EnemyHandler>();
	private AnimatedWalker animatedWalker;
	private UnitAnimType attackUnitAnim;
	private int currentPathIndex;
	private Func<IEnemyTargetable> getEnemyTarget;
	private int health;
	private UnitAnimType hitUnitAnim;

	private UnitAnimType idleUnitAnim;
	private float pathfindingTimer;
	private List<Vector3> pathVectorList;

	private State state;
	private V_UnitAnimation unitAnimation;

	private V_UnitSkeleton unitSkeleton;
	private UnitAnimType walkUnitAnim;

	private void Awake()
	{
		enemyHandlerList.Add(this);
		health = 40;
		state = State.Normal;
	}

	private void Start()
	{
		var bodyTransform = transform.Find("Body");
		unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint,
				mesh => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
		unitAnimation = new V_UnitAnimation(unitSkeleton);

		idleUnitAnim = UnitAnimType.GetUnitAnimType("dMinion_Idle");
		walkUnitAnim = UnitAnimType.GetUnitAnimType("dMinion_Walk");
		hitUnitAnim = null;
		attackUnitAnim = UnitAnimType.GetUnitAnimType("dMinion_Attack");

		idleUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Idle");
		walkUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Walk");
		hitUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Hit");
		attackUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Punch");

		//animatedWalker = new AnimatedWalker(unitAnimation, UnitAnimType.GetUnitAnimType("dMinion_Idle"), UnitAnimType.GetUnitAnimType("dMinion_Walk"), 1f, 1f);
		animatedWalker
				= new AnimatedWalker(unitAnimation, idleUnitAnim, walkUnitAnim,
						1f, 1f);

		bodyTransform.GetComponent<MeshRenderer>().material
				= GameAssets.i.m_MarineSpriteSheet;
		//unitAnimation.PlayAnimForced(UnitAnimType.GetUnitAnimType("dBareHands_AttackPose"), 1f, null, null, null);
		//state = State.Busy;
	}

	private void Update()
	{
		unitSkeleton.Update(Time.deltaTime);
		pathfindingTimer -= Time.deltaTime;

		switch (state)
		{
			case State.Normal:
				HandleMovement();
				FindTarget();
				break;
			case State.Busy:
				break;
		}
	}

	public static EnemyHandler GetClosestEnemy(Vector3 position, float maxRange)
	{
		EnemyHandler closest = null;
		foreach (var enemyHandler in enemyHandlerList)
		{
			if (enemyHandler.IsDead()) continue;
			if (Vector3.Distance(position, enemyHandler.GetPosition()) <=
			    maxRange)
			{
				if (closest == null) { closest = enemyHandler; }
				else
				{
					if (Vector3.Distance(position, enemyHandler.GetPosition()) <
					    Vector3.Distance(position, closest.GetPosition()))
						closest = enemyHandler;
				}
			}
		}
		return closest;
	}


	public static EnemyHandler Create(Vector3 position)
	{
		var enemyTransform
				= Instantiate(GameAssets.i.pfEnemy, position,
						Quaternion.identity);

		var enemyHandler = enemyTransform.GetComponent<EnemyHandler>();

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
				state = State.Busy;
				unitAnimation.PlayAnimForced(attackUnitAnim, 1f, unitAnim =>
				{
					// Attack complete
					state = State.Normal;
				}, trigger =>
				{
					// Damage Player
					getEnemyTarget().Damage(this);
				}, null);
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
		return health <= 0;
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public void Damage(IEnemyTargetable attacker)
	{
		var bloodDir = (GetPosition() - attacker.GetPosition()).normalized;
		//Blood_Handler.SpawnBlood(GetPosition(), bloodDir);
		health--;
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
				unitAnimation.PlayAnimForced(hitUnitAnim,
						bloodDir * (Vector2.one * -1f), 1f,
						unitAnim => { state = State.Normal; }, null, null);
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

	public void SetTargetPosition(Vector3 targetPosition)
	{
		currentPathIndex = 0;
		pathVectorList = GridPathfinding.instance
				.GetPathRouteWithShortcuts(GetPosition(), targetPosition)
				.pathVectorList;
		if (pathVectorList != null && pathVectorList.Count > 1)
			pathVectorList.RemoveAt(0);
	}

	public interface IEnemyTargetable
	{
		Vector3 GetPosition();
		void Damage(EnemyHandler attacker);
	}
	private enum State
	{
		Normal,
		Busy
	}
}
