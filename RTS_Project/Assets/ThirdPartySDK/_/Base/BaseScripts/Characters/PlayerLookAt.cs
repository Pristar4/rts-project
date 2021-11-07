#region Info
// -----------------------------------------------------------------------
// PlayerLookAt.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class PlayerLookAt : MonoBehaviour
{

	private const float SPEED = 50f;
	private Vector3 lookAtPosition;

	private CharacterLookAt_Base playerBase;

	private void Awake()
	{
		playerBase = gameObject.GetComponent<CharacterLookAt_Base>();
	}

	private void Update()
	{
		//HandleLookAtMouse();
		HandleMovement();

		/*
		if (Input.GetMouseButtonDown(0)) {
		    transform.Find("Aim").GetComponent<Animator>().SetTrigger("Shoot");
		    ShellParticleSystemHandler.Instance.SpawnShell(GetPosition(), (lookAtPosition - GetPosition()).normalized);
		}
		*/
	}

	private void HandleLookAtMouse()
	{
		lookAtPosition = UtilsClass.GetMouseWorldPosition();
	}

	public void SetLookAtPosition(Vector3 lookAtPosition)
	{
		this.lookAtPosition = lookAtPosition;
	}

	private void HandleMovement()
	{
		var moveX = 0f;
		var moveY = 0f;

		if (Input.GetKey(KeyCode.W)) moveY = +1f;
		if (Input.GetKey(KeyCode.S)) moveY = -1f;
		if (Input.GetKey(KeyCode.A)) moveX = -1f;
		if (Input.GetKey(KeyCode.D)) moveX = +1f;

		var lookAtDir = (lookAtPosition - GetPosition()).normalized;

		var moveDir = new Vector3(moveX, moveY).normalized;

		var isIdle = moveX == 0 && moveY == 0;
		if (isIdle)
		{
			playerBase.PlayFeetIdleAnim(moveDir);
			playerBase.PlayBodyHeadIdleAnim(lookAtDir);
		}
		else
		{
			playerBase.PlayFeetWalkAnim(moveDir);
			playerBase.PlayBodyHeadWalkAnim(lookAtDir);
			transform.position += moveDir * SPEED * Time.deltaTime;
		}
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}
}
