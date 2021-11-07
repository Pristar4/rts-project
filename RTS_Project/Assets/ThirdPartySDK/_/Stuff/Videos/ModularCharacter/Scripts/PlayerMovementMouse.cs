#region Info
// -----------------------------------------------------------------------
// PlayerMovementMouse.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class PlayerMovementMouse : MonoBehaviour
{

	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
			GetComponent<IMovePosition>()
					.SetMovePosition(UtilsClass.GetMouseWorldPosition());
	}
}
