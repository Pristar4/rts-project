#region Info
// -----------------------------------------------------------------------
// MovePositionDirect.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class MovePositionDirect : MonoBehaviour, IMovePosition
{

	private Vector3 movePosition;

	private void Awake()
	{
		movePosition = transform.position;
	}

	private void Update()
	{
		var moveDir = (movePosition - transform.position).normalized;
		if (Vector3.Distance(movePosition, transform.position) < 1f)
			moveDir = Vector3.zero; // Stop moving when near
		GetComponent<IMoveVelocity>().SetVelocity(moveDir);
	}

	public void SetMovePosition(Vector3 movePosition)
	{
		this.movePosition = movePosition;
	}
}
