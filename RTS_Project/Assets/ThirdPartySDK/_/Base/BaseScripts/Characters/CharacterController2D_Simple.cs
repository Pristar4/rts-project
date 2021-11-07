#region Info
// -----------------------------------------------------------------------
// CharacterController2D_Simple.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class CharacterController2D_Simple : MonoBehaviour
{

	private const float MOVE_SPEED = 60f;

	[SerializeField] private LayerMask dashLayerMask;

	private Character_Base characterBase;
	private bool isDashButtonDown;
	private Vector3 moveDir;
	private Rigidbody2D rigidbody2D;

	private void Awake()
	{
		characterBase = GetComponent<Character_Base>();
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		var moveX = 0f;
		var moveY = 0f;

		if (Input.GetKey(KeyCode.W)) moveY = +1f;
		if (Input.GetKey(KeyCode.S)) moveY = -1f;
		if (Input.GetKey(KeyCode.A)) moveX = -1f;
		if (Input.GetKey(KeyCode.D)) moveX = +1f;

		moveDir = new Vector3(moveX, moveY).normalized;
		characterBase.PlayMoveAnim(moveDir);

		if (Input.GetKeyDown(KeyCode.Space)) isDashButtonDown = true;
	}

	private void FixedUpdate()
	{
		rigidbody2D.velocity = moveDir * MOVE_SPEED;

		if (isDashButtonDown)
		{
			var dashAmount = 50f;
			var dashPosition = transform.position + moveDir * dashAmount;

			var raycastHit2d = Physics2D.Raycast(transform.position, moveDir,
					dashAmount, dashLayerMask);
			if (raycastHit2d.collider != null)
				dashPosition = raycastHit2d.point;

			// Spawn visual effect
			DashEffect.CreateDashEffect(transform.position, moveDir,
					Vector3.Distance(transform.position, dashPosition));

			rigidbody2D.MovePosition(dashPosition);
			isDashButtonDown = false;
		}
	}
}
