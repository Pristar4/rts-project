#region Info
// -----------------------------------------------------------------------
// CharacterWaypointsHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using UnityEngine;
using V_AnimationSystem;
#endregion

/*
 * Character moves between waypoints
 * */
public class CharacterWaypointsHandler : MonoBehaviour
{

	private const float speed = 30f;

	[SerializeField] private List<Vector3> waypointList;
	[SerializeField] private List<float> waitTimeList;

	[SerializeField] private string idleAnimation = "dZombie_Idle";
	[SerializeField] private string walkAnimation = "dZombie_Walk";

	[SerializeField] private float idleFrameRate = 1f;
	[SerializeField] private float walkFrameRate = 1f;
	[SerializeField] private Vector3 defaultAimDirection;

	[SerializeField] private PlayerMovement player;
	private AnimatedWalker animatedWalker;
	private UnitAnimType attackUnitAnim;
	private Vector3 lastMoveDir;

	private State state;
	private V_UnitAnimation unitAnimation;

	private V_UnitSkeleton unitSkeleton;
	private float waitTimer;
	private int waypointIndex;

	private void Start()
	{
		var bodyTransform = transform.Find("Body");
		unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint,
				mesh => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
		unitAnimation = new V_UnitAnimation(unitSkeleton);
		animatedWalker = new AnimatedWalker(unitAnimation,
				UnitAnimType.GetUnitAnimType(idleAnimation),
				UnitAnimType.GetUnitAnimType(walkAnimation), idleFrameRate,
				walkFrameRate);
		state = State.Waiting;
		waitTimer = waitTimeList[0];
		lastMoveDir = defaultAimDirection;
		attackUnitAnim = UnitAnimType.GetUnitAnimType("dMarine_Attack");
	}

	private void Update()
	{
		switch (state)
		{
			default:
			case State.Waiting:
			case State.Moving:
				HandleMovement();
				FindTargetPlayer();
				break;
			case State.AttackingPlayer:
				AttackPlayer();
				break;
			case State.Busy:
				break;
		}
		unitSkeleton.Update(Time.deltaTime);
	}

	private void FindTargetPlayer()
	{
		var viewDistance = 50f;
		var fov = 180f;
		if (Vector3.Distance(GetPosition(), player.GetPosition()) <
		    viewDistance)
		{
			// Player inside viewDistance
			var dirToPlayer = (player.GetPosition() - GetPosition()).normalized;
			if (Vector3.Angle(GetAimDir(), dirToPlayer) < fov / 2f)
			{
				// Player inside Field of View
				var raycastHit2D
						= Physics2D.Raycast(GetPosition(), dirToPlayer,
								viewDistance);
				if (raycastHit2D.collider != null) // Hit something
					if (raycastHit2D.collider.gameObject
							    .GetComponent<PlayerMovement>() !=
					    null) // Hit Player
						StartAttackingPlayer();
			}
		}
	}

	public void StartAttackingPlayer()
	{
		AttackPlayer();
	}

	private void AttackPlayer()
	{
		state = State.Busy;

		var targetPosition = player.GetPosition();
		var dirToTarget = (targetPosition - GetPosition()).normalized;
		lastMoveDir = dirToTarget;

		unitAnimation.PlayAnimForced(attackUnitAnim, dirToTarget, 2f, unitAnim
				=>
		{
			// Attack complete
			if (player.IsDead())
				state = State.Moving;
			else
				state = State.AttackingPlayer;
		}, trigger =>
		{
			// Damage Player
			player.Damage(this);
		}, null);

		var gunEndPointPosition
				= unitSkeleton.GetBodyPartPosition("MuzzleFlash");
		Shoot_Flash.AddFlash(gunEndPointPosition);
		WeaponTracer.Create(gunEndPointPosition, player.GetPosition());
	}

	private void HandleMovement()
	{
		switch (state)
		{
			case State.Waiting:
				waitTimer -= Time.deltaTime;
				animatedWalker.SetMoveVector(Vector3.zero);
				if (waitTimer <= 0f) state = State.Moving;
				break;
			case State.Moving:
				var waypoint = waypointList[waypointIndex];

				var waypointDir = (waypoint - transform.position).normalized;

				var distanceBefore
						= Vector3.Distance(transform.position, waypoint);
				animatedWalker.SetMoveVector(waypointDir);
				transform.position
						= transform.position +
						  waypointDir * speed * Time.deltaTime;
				var distanceAfter
						= Vector3.Distance(transform.position, waypoint);

				var arriveDistance = .1f;
				if (distanceAfter < arriveDistance ||
				    distanceBefore <= distanceAfter)
				{
					// Go to next waypoint
					waitTimer = waitTimeList[waypointIndex];
					waypointIndex = (waypointIndex + 1) % waypointList.Count;
					state = State.Waiting;
				}
				break;
		}
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public Vector3 GetAimDir()
	{
		return lastMoveDir;
	}

	private enum State
	{
		Waiting,
		Moving,
		AttackingPlayer,
		Busy
	}
}
