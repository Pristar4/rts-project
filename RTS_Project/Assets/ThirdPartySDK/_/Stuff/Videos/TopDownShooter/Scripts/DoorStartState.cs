#region Info
// -----------------------------------------------------------------------
// DoorStartState.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class DoorStartState : MonoBehaviour
{

	[SerializeField] private bool startOpen;
	[SerializeField] private DoorAnims.ColorName doorColor;

	private void Start()
	{
		var doorAnims = GetComponent<DoorAnims>();
		if (startOpen)
			doorAnims.OpenDoor();
		else
			doorAnims.CloseDoor();

		doorAnims.SetColor(doorColor);
	}
}
