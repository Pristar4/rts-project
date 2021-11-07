#region Info
// -----------------------------------------------------------------------
// UI_ItemSlot.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
using UnityEngine.EventSystems;
#endregion
public class UI_ItemSlot : MonoBehaviour, IDropHandler
{

	private Action onDropAction;

	public void OnDrop(PointerEventData eventData)
	{
		UI_ItemDrag.Instance.Hide();
		onDropAction();
	}

	public void SetOnDropAction(Action onDropAction)
	{
		this.onDropAction = onDropAction;
	}
}
