#region Info
// -----------------------------------------------------------------------
// PlayerPlatformer.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

/*
 * Simple Jump
 * */
public class PlayerPlatformer : MonoBehaviour
{

	private static PlayerPlatformer instance;

	[SerializeField] private LayerMask platformsLayerMask;
	private BoxCollider2D boxCollider2d;
	private bool isDead;
	private Player_Base playerBase;
	private Rigidbody2D rigidbody2d;
	private bool waitForStart;

	private void Awake()
	{
		instance = this;
		playerBase = gameObject.GetComponent<Player_Base>();
		rigidbody2d = transform.GetComponent<Rigidbody2D>();
		boxCollider2d = transform.GetComponent<BoxCollider2D>();
		isDead = false;
	}

	private void Start()
	{
		playerBase.PlayMoveAnim(Vector2.right);
	}

	private void Update()
	{
		if (isDead) return;

		if (IsGrounded())
			if (Input.GetKeyDown(KeyCode.Space) ||
			    Input.GetKeyDown(KeyCode.UpArrow))
			{
				var jumpVelocity = 100f;
				rigidbody2d.velocity = Vector2.up * jumpVelocity;
			}

		HandleMovement();

		// Set Animations
		if (IsGrounded())
		{
			if (rigidbody2d.velocity.x == 0)
				playerBase.PlayIdleAnim();
			else
				playerBase.PlayMoveAnim(new Vector2(rigidbody2d.velocity.x,
						0f));
		}
		else { playerBase.PlayJumpAnim(rigidbody2d.velocity); }

		if (rigidbody2d.velocity.y < -300f) // Falling way too fast, dead
			Die();
	}

	public event EventHandler OnDead;

	private bool IsGrounded()
	{
		var raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center,
				boxCollider2d.bounds.size, 0f, Vector2.down, 1f,
				platformsLayerMask);
		return raycastHit2d.collider != null;
	}

	private void HandleMovement()
	{
		var moveX = 0f;

		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			moveX = -1f;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			moveX = +1f;

		var moveSpeed = 40f;
		rigidbody2d.velocity
				= new Vector2(moveX * moveSpeed, rigidbody2d.velocity.y);
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	private void Die()
	{
		isDead = true;
		rigidbody2d.velocity = Vector3.zero;
		if (OnDead != null) OnDead(this, EventArgs.Empty);
	}

	public static void Die_Static()
	{
		instance.Die();
	}
}
