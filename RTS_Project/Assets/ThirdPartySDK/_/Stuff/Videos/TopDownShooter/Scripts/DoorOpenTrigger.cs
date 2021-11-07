#region Info
// -----------------------------------------------------------------------
// DoorOpenTrigger.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion
public class DoorOpenTrigger : MonoBehaviour
{

	[SerializeField] private CaptureOnTriggerEnter2D captureOnTriggerEnter2D;
	[SerializeField] private DoorAnims doorAnims;

	private void Awake()
	{
		captureOnTriggerEnter2D.OnPlayerTriggerEnter2D
				+= DoorOpenTrigger_OnPlayerTriggerEnter2D;
	}

	private void DoorOpenTrigger_OnPlayerTriggerEnter2D(
			object sender, EventArgs e)
	{
		doorAnims.SetColor(DoorAnims.ColorName.Green);
		doorAnims.OpenDoor();
		captureOnTriggerEnter2D.OnPlayerTriggerEnter2D
				-= DoorOpenTrigger_OnPlayerTriggerEnter2D;
	}
}
