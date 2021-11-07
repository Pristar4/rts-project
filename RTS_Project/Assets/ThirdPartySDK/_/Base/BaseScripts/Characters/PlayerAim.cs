#region Info
// -----------------------------------------------------------------------
// PlayerAim.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class PlayerAim : MonoBehaviour, Enemy.IEnemyTargetable,
		EnemyHandler.IEnemyTargetable, EnemyAim.IEnemyTargetable
{

	private const float SPEED = 50f;
	private const float FIRE_RATE = .15f;

	public static PlayerAim instance;
	private HealthSystem healthSystem;
	private float nextShootTime;

	private CharacterAim_Base playerBase;
	private State state;

	private void Awake()
	{
		instance = this;
		playerBase = gameObject.GetComponent<CharacterAim_Base>();
		healthSystem = new HealthSystem(100);
		SetStateNormal();
	}

	private void Update()
	{
		switch (state)
		{
			case State.Normal:
				HandleAimShooting();
				HandleMovement();
				break;
		}
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public void Damage(Enemy enemy) { }

	public void Damage(EnemyHandler enemy) { }

	public void Damage(EnemyAim attacker)
	{
		var bloodDir = (GetPosition() - attacker.GetPosition()).normalized;
		Blood_Handler.SpawnBlood(GetPosition(), bloodDir);

		healthSystem.Damage(1);
		if (IsDead())
		{
			FlyingBody.Create(GameAssets.i.pfEnemyFlyingBody, GetPosition(),
					bloodDir);
			playerBase.DestroySelf();
			Destroy(gameObject);
		}
		else
		{
			// Knockback
			transform.position += bloodDir * 2.5f;
		}
	}

	private void SetStateNormal()
	{
		state = State.Normal;
	}

	private void HandleAimShooting()
	{
		var targetPosition = UtilsClass.GetMouseWorldPosition();
		playerBase.SetAimTarget(targetPosition);

		if (Input.GetMouseButton(0) && Time.time >= nextShootTime)
		{
			nextShootTime = Time.time + FIRE_RATE;
			playerBase.ShootTarget(targetPosition);
		}
	}

	private void HandleMovement()
	{
		var moveX = 0f;
		var moveY = 0f;

		if (Input.GetKey(KeyCode.W)) moveY = +1f;
		if (Input.GetKey(KeyCode.S)) moveY = -1f;
		if (Input.GetKey(KeyCode.A)) moveX = -1f;
		if (Input.GetKey(KeyCode.D)) moveX = +1f;

		var moveDir = new Vector3(moveX, moveY).normalized;
		var isIdle = moveX == 0 && moveY == 0;
		if (isIdle) { playerBase.PlayIdleAnim(); }
		else
		{
			playerBase.PlayMoveAnim(moveDir);
			transform.position += moveDir * SPEED * Time.deltaTime;
		}
	}

	public bool IsDead()
	{
		return healthSystem.IsDead();
	}

	private enum State
	{
		Normal
	}
}
