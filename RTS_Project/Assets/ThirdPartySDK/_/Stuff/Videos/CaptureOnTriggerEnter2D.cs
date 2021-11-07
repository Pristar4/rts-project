#region Info
// -----------------------------------------------------------------------
// CaptureOnTriggerEnter2D.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion
public class CaptureOnTriggerEnter2D : MonoBehaviour
{

	private void OnTriggerEnter2D(Collider2D collider)
	{
		OnCapturedTriggerEnter2D?.Invoke(collider, EventArgs.Empty);

		var player = collider.GetComponent<Player>();
		if (player != null)
			OnPlayerTriggerEnter2D?.Invoke(player, EventArgs.Empty);
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		OnCapturedTriggerExit2D?.Invoke(collider, EventArgs.Empty);

		var player = collider.GetComponent<Player>();
		if (player != null)
			OnPlayerTriggerExit2D?.Invoke(player, EventArgs.Empty);
	}

	public event EventHandler OnCapturedTriggerEnter2D;
	public event EventHandler OnPlayerTriggerEnter2D;
	public event EventHandler OnCapturedTriggerExit2D;
	public event EventHandler OnPlayerTriggerExit2D;
}
