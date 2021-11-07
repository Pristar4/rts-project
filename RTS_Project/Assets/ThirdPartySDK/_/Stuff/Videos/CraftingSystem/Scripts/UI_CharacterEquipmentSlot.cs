#region Info
// -----------------------------------------------------------------------
// UI_CharacterEquipmentSlot.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
using UnityEngine.EventSystems;
#endregion
public class UI_CharacterEquipmentSlot : MonoBehaviour, IDropHandler
{

	public void OnDrop(PointerEventData eventData)
	{
		UI_ItemDrag.Instance.Hide();
		var item = UI_ItemDrag.Instance.GetItem();
		OnItemDropped?.Invoke(this, new OnItemDroppedEventArgs { item = item });
	}

	public event EventHandler<OnItemDroppedEventArgs> OnItemDropped;
	public class OnItemDroppedEventArgs : EventArgs
	{
		public Item item;
	}
}
