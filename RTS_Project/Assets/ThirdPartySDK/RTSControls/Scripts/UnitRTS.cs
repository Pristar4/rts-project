#region Info
// -----------------------------------------------------------------------
// UnitRTS.cs
//
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class UnitRTS : MonoBehaviour
{
	private IMovePosition movePosition;

	private GameObject selectedGameObject;

	private void Awake()
	{
		selectedGameObject = transform.Find("Selected").gameObject;
		movePosition = GetComponent<IMovePosition>();
		SetSelectedVisible(false);
	}

	public void SetSelectedVisible(bool visible)
	{
		selectedGameObject.SetActive(visible);
	}

	public void MoveTo(Vector3 targetPosition)
	{
		movePosition.SetMovePosition(targetPosition);
	}
}
