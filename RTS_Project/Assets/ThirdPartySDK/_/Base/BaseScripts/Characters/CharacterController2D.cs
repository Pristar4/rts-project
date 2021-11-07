#region Info
// -----------------------------------------------------------------------
// CharacterController2D.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class CharacterController2D : MonoBehaviour
{

	private const float MOVE_SPEED = 60f;

	[SerializeField] private LayerMask dashLayerMask;

	private Character_Base characterBase;
	private bool isDashButtonDown;
	private Vector3 lastMoveDir;
	private Vector3 moveDir;
	private Rigidbody2D rigidbody2D;
	private Vector3 rollDir;
	private float rollSpeed;
	private State state;

	private void Awake()
	{
		characterBase = GetComponent<Character_Base>();
		rigidbody2D = GetComponent<Rigidbody2D>();
		state = State.Normal;
	}

	private void Update()
	{
		switch (state)
		{
			case State.Normal:
				var moveX = 0f;
				var moveY = 0f;

				if (Input.GetKey(KeyCode.W)) moveY = +1f;
				if (Input.GetKey(KeyCode.S)) moveY = -1f;
				if (Input.GetKey(KeyCode.A)) moveX = -1f;
				if (Input.GetKey(KeyCode.D)) moveX = +1f;

				moveDir = new Vector3(moveX, moveY).normalized;
				if (moveX != 0 || moveY != 0) // Not idle
					lastMoveDir = moveDir;
				characterBase.PlayMoveAnim(moveDir);

				if (Input.GetKeyDown(KeyCode.F)) isDashButtonDown = true;

				if (Input.GetKeyDown(KeyCode.Space))
				{
					rollDir = lastMoveDir;
					rollSpeed = 250f;
					state = State.Rolling;
					characterBase.PlayRollAnimation(rollDir);
				}
				break;
			case State.Rolling:
				var rollSpeedDropMultiplier = 5f;
				rollSpeed -= rollSpeed * rollSpeedDropMultiplier *
				             Time.deltaTime;

				var rollSpeedMinimum = 50f;
				if (rollSpeed < rollSpeedMinimum) state = State.Normal;
				break;
		}
	}

	private void FixedUpdate()
	{
		switch (state)
		{
			case State.Normal:
				rigidbody2D.velocity = moveDir * MOVE_SPEED;

				if (isDashButtonDown)
				{
					var dashAmount = 50f;
					var dashPosition
							= transform.position + lastMoveDir * dashAmount;

					var raycastHit2d = Physics2D.Raycast(transform.position,
							lastMoveDir,
							dashAmount, dashLayerMask);
					if (raycastHit2d.collider != null)
						dashPosition = raycastHit2d.point;

					// Spawn visual effect
					DashEffect.CreateDashEffect(transform.position, lastMoveDir,
							Vector3.Distance(transform.position, dashPosition));

					rigidbody2D.MovePosition(dashPosition);
					isDashButtonDown = false;
				}
				break;
			case State.Rolling:
				rigidbody2D.velocity = rollDir * rollSpeed;
				break;
		}
	}

	private enum State
	{
		Normal,
		Rolling
	}
}
