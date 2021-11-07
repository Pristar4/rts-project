#region Info
// -----------------------------------------------------------------------
// PlayerCharacterDashRoll.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class PlayerCharacterDashRoll : MonoBehaviour
{

	[SerializeField] private Transform pfDashEffect;
	private Vector3 lastMoveDir;
	private PlayerCharacterDashRoll_Base playerCharacterBase;
	private Vector3 slideDir;
	private float slideSpeed;

	private State state;

	private void Awake()
	{
		playerCharacterBase
				= gameObject.GetComponent<PlayerCharacterDashRoll_Base>();
		state = State.Normal;
	}

	private void Update()
	{
		switch (state)
		{
			case State.Normal:
				HandleMovement();
				HandleDash();
				HandleDodgeRoll();
				break;
			case State.DodgeRollSliding:
				HandleDodgeRollSliding();
				break;
		}
	}

	private void HandleMovement()
	{
		var speed = 50f;
		var moveX = 0f;
		var moveY = 0f;

		if (Input.GetKey(KeyCode.W)) moveY = +1f;
		if (Input.GetKey(KeyCode.S)) moveY = -1f;
		if (Input.GetKey(KeyCode.A)) moveX = -1f;
		if (Input.GetKey(KeyCode.D)) moveX = +1f;

		var isIdle = moveX == 0 && moveY == 0;
		if (isIdle) { playerCharacterBase.PlayIdleAnimation(lastMoveDir); }
		else
		{
			var moveDir = new Vector3(moveX, moveY).normalized;

			if (TryMove(moveDir, speed * Time.deltaTime))
				playerCharacterBase.PlayWalkingAnimation(lastMoveDir);
			else
				playerCharacterBase.PlayIdleAnimation(lastMoveDir);
		}
	}

	private bool CanMove(Vector3 dir, float distance)
	{
		return Physics2D.Raycast(transform.position, dir, distance).collider ==
		       null;
	}

	private bool TryMove(Vector3 baseMoveDir, float distance)
	{
		var moveDir = baseMoveDir;
		var canMove = CanMove(moveDir, distance);
		if (!canMove)
		{
			// Cannot move diagonally
			moveDir = new Vector3(baseMoveDir.x, 0f).normalized;
			canMove = moveDir.x != 0f && CanMove(moveDir, distance);
			if (!canMove)
			{
				// Cannot move horizontally
				moveDir = new Vector3(0f, baseMoveDir.y).normalized;
				canMove = moveDir.y != 0f && CanMove(moveDir, distance);
			}
		}

		if (canMove)
		{
			lastMoveDir = moveDir;
			transform.position += moveDir * distance;
			return true;
		}
		return false;
	}

	private void HandleDash()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			var dashDistance = 30f;
			var beforeDashPosition = transform.position;
			if (TryMove(lastMoveDir, dashDistance))
			{
				var dashEffectTransform = Instantiate(pfDashEffect,
						beforeDashPosition,
						Quaternion.identity);
				dashEffectTransform.eulerAngles = new Vector3(0, 0,
						UtilsClass.GetAngleFromVectorFloat(lastMoveDir));
				var dashEffectWidth = 30f;
				dashEffectTransform.localScale
						= new Vector3(dashDistance / dashEffectWidth, 1f, 1f);
			}
		}
	}

	private void HandleDodgeRoll()
	{
		if (Input.GetMouseButtonDown(1))
		{
			state = State.DodgeRollSliding;
			slideDir = (UtilsClass.GetMouseWorldPosition() - transform.position)
					.normalized;
			slideSpeed = 500f;
			playerCharacterBase.PlayDodgeAnimation(slideDir);
		}
	}

	private void HandleDodgeRollSliding()
	{
		TryMove(slideDir, slideSpeed * Time.deltaTime);
		//transform.position += slideDir * slideSpeed * Time.deltaTime;

		slideSpeed -= slideSpeed * 10f * Time.deltaTime;
		if (slideSpeed < 10f) state = State.Normal;
	}
	private enum State
	{
		Normal,
		DodgeRollSliding
	}
}
