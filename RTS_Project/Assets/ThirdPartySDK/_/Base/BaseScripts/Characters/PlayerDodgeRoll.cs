#region Info
// -----------------------------------------------------------------------
// PlayerDodgeRoll.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class PlayerDodgeRoll : MonoBehaviour
{

	private const float ROLL_SPEED = 250f;

	private PlayerMain playerMain;
	private Vector3 rollDir;
	private float rollSpeed;

	private State state;

	private void Awake()
	{
		playerMain = GetComponent<PlayerMain>();
		state = State.Normal;
	}

	private void Update()
	{
		switch (state)
		{
			default:
			case State.Normal:
				HandleInput();
				break;
			case State.Rolling:
				HandleRolling();
				break;
		}
	}

	private void FixedUpdate()
	{
		switch (state)
		{
			case State.Rolling:
				playerMain.PlayerRigidbody2D.velocity = rollDir * rollSpeed;
				break;
		}
	}

	private void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			rollDir = playerMain.PlayerMovementHandler.GetLastMoveDir();
			rollSpeed = ROLL_SPEED;
			state = State.Rolling;
			playerMain.PlayerMovementHandler.Disable();
			//playerMain.PlayerSwapAimNormal.PlayDodgeAnimation(rollDir);
			//playerMain.Player.Dodged();
		}
	}

	private void HandleRolling()
	{
		var rollSpeedDropMultiplier = 5f;
		rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

		var rollSpeedMinimum = 50f;
		if (rollSpeed < rollSpeedMinimum)
		{
			state = State.Normal;
			playerMain.PlayerMovementHandler.Enable();
			//playerMain.PlayerSwapAimNormal.SetWeapon();
		}
	}

	private enum State
	{
		Normal,
		Rolling
	}

	/*
	private void HandleDash() {
	    if (Input.GetKeyDown(KeyCode.Space)) {
	        float dashDistance = 30f;
	        Vector3 beforeDashPosition = transform.position;
	        if (TryMove(lastMoveDir, dashDistance)) {
	            Transform dashEffectTransform = Instantiate(pfDashEffect, beforeDashPosition, Quaternion.identity);
	            dashEffectTransform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(lastMoveDir));
	            float dashEffectWidth = 30f;
	            dashEffectTransform.localScale = new Vector3(dashDistance / dashEffectWidth, 1f, 1f);
	        }
	    }
	}
	*/
}
