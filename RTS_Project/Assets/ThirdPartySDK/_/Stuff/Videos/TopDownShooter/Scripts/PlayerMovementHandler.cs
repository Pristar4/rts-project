#region Info
// -----------------------------------------------------------------------
// PlayerMovementHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion

namespace TopDownShooter
{
	public class PlayerMovementHandler : MonoBehaviour
	{

		private const float SPEED = 50f;
		private Vector3 lastMoveDir;

		private Vector3 moveDir;

		private PlayerMain playerMain;

		private void Awake()
		{
			playerMain = GetComponent<PlayerMain>();
		}

		private void Update()
		{
			HandleMovement();
		}

		private void FixedUpdate()
		{
			playerMain.PlayerRigidbody2D.velocity = moveDir * SPEED;
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

			var isIdle = moveX == 0 && moveY == 0;
			if (isIdle) { playerMain.PlayerSwapAimNormal.PlayIdleAnim(); }
			else
			{
				lastMoveDir = moveDir;
				playerMain.PlayerSwapAimNormal.PlayMoveAnim(moveDir);
			}
		}

		public void Enable()
		{
			enabled = true;
		}

		public void Disable()
		{
			enabled = false;
			playerMain.PlayerRigidbody2D.velocity = Vector3.zero;
		}

		public Vector3 GetLastMoveDir()
		{
			return lastMoveDir;
		}
	}
}
