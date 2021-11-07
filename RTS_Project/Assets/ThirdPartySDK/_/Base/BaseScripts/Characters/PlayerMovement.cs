#region Info
// -----------------------------------------------------------------------
// PlayerMovement.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion

/*
 * Player movement with WASD
 * */
public class PlayerMovement : MonoBehaviour, EnemyHandler.IEnemyTargetable
{

	private const float SPEED = 50f;

	public static PlayerMovement instance;

	[SerializeField] private MaterialTintColor materialTintColor;
	private int health;
	private Vector3 moveDir;

	private Player_Base playerBase;
	private Rigidbody2D playerRigidbody2D;
	private State state;

	private void Awake()
	{
		instance = this;
		playerBase = gameObject.GetComponent<Player_Base>();
		playerRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
		SetStateNormal();
		health = 4;
	}

	private void Update()
	{
		switch (state)
		{
			case State.Normal:
				HandleMovement();
				break;
		}
	}

	private void FixedUpdate()
	{
		var isIdle = moveDir.x == 0 && moveDir.y == 0;
		if (isIdle) { playerBase.PlayIdleAnim(); }
		else
		{
			playerBase.PlayMoveAnim(moveDir);
			//transform.position += moveDir * SPEED * Time.deltaTime;
			playerRigidbody2D.MovePosition(transform.position +
			                               moveDir * SPEED *
			                               Time.fixedDeltaTime);
		}

	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public void Damage(EnemyHandler enemyHandler)
	{
		Damage(enemyHandler.GetPosition());
	}

	private void SetStateNormal()
	{
		state = State.Normal;
	}

	private void HandleMovement()
	{
		var moveX = 0f;
		var moveY = 0f;

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			moveY = +1f;
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			moveY = -1f;
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			moveX = -1f;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			moveX = +1f;

		moveDir = new Vector3(moveX, moveY).normalized;
	}

	private void DamageFlash()
	{
		materialTintColor.SetTintColor(new Color(1, 0, 0, 1f));
	}

	public void DamageKnockback(Vector3 knockbackDir, float knockbackDistance)
	{
		transform.position += knockbackDir * knockbackDistance;
		DamageFlash();
	}

	public void Damage(CharacterWaypointsHandler enemyHandler)
	{
		Damage(enemyHandler.GetPosition());
	}

	public void Damage(Vector3 attackerPosition)
	{
		var bloodDir = (GetPosition() - attackerPosition).normalized;
		Blood_Handler.SpawnBlood(GetPosition(), bloodDir);
		// Knockback
		transform.position += bloodDir * 1.5f;
		health--;
		if (health == 0)
		{
			FlyingBody.Create(GameAssets.i.pfEnemyFlyingBody, GetPosition(),
					bloodDir);
			gameObject.SetActive(false);
			//transform.Find("Body").gameObject.SetActive(false);
		}
	}

	public bool IsDead()
	{
		return health <= 0;
	}

	private enum State
	{
		Normal
	}
}
