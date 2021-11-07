﻿#region Info
// -----------------------------------------------------------------------
// PlayerPositionMovement.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion

/*
 * Player movement with TargetPosition
 * */
public class PlayerPositionMovement : MonoBehaviour
{

	private const float SPEED = 50f;

	private Player_Base playerBase;
	private Vector3 targetPosition;

	private void Awake()
	{
		playerBase = gameObject.GetComponent<Player_Base>();
	}

	private void Update()
	{
		HandleMovement();

		if (Input.GetMouseButtonDown(0))
			SetTargetPosition(UtilsClass.GetMouseWorldPosition());
	}

	private void HandleMovement()
	{
		if (Vector3.Distance(transform.position, targetPosition) > 1f)
		{
			var moveDir = (targetPosition - transform.position).normalized;

			playerBase.PlayMoveAnim(moveDir);
			transform.position += moveDir * SPEED * Time.deltaTime;
		}
		else { playerBase.PlayIdleAnim(); }
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public void SetTargetPosition(Vector3 targetPosition)
	{
		targetPosition.z = 0f;
		this.targetPosition = targetPosition;
	}
}
