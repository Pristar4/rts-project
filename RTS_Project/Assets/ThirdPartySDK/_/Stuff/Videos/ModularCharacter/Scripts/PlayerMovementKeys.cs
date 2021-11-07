#region Info
// -----------------------------------------------------------------------
// PlayerMovementKeys.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class PlayerMovementKeys : MonoBehaviour
{

	private void Update()
	{
		var moveX = 0f;
		var moveY = 0f;

		if (Input.GetKey(KeyCode.W)) moveY = +1f;
		if (Input.GetKey(KeyCode.S)) moveY = -1f;
		if (Input.GetKey(KeyCode.A)) moveX = -1f;
		if (Input.GetKey(KeyCode.D)) moveX = +1f;

		var moveVector = new Vector3(moveX, moveY).normalized;
		GetComponent<IMoveVelocity>().SetVelocity(moveVector);
	}
}
