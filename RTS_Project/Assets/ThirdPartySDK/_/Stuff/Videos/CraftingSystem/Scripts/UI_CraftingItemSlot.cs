#region Info
// -----------------------------------------------------------------------
// UI_CraftingItemSlot.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
using UnityEngine.EventSystems;
#endregion
public class UI_CraftingItemSlot : MonoBehaviour, IDropHandler
{

	private int x;
	private int y;

	public void OnDrop(PointerEventData eventData)
	{
		UI_ItemDrag.Instance.Hide();
		var item = UI_ItemDrag.Instance.GetItem();
		OnItemDropped?.Invoke(this,
				new OnItemDroppedEventArgs { item = item, x = x, y = y });
	}

	public event EventHandler<OnItemDroppedEventArgs> OnItemDropped;

	public void SetXY(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	public class OnItemDroppedEventArgs : EventArgs
	{
		public Item item;
		public int x;
		public int y;
	}
}
