#region Info
// -----------------------------------------------------------------------
// InventorySimple.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
#endregion
public class InventorySimple
{

	private readonly List<Item> itemList;
	private readonly Action<Item> useItemAction;

	public InventorySimple(Action<Item> useItemAction)
	{
		this.useItemAction = useItemAction;
		itemList = new List<Item>();

		AddItem(new Item { itemType = Item.ItemType.Sword, amount = 1 });
		AddItem(new Item { itemType = Item.ItemType.HealthPotion, amount = 1 });
		AddItem(new Item { itemType = Item.ItemType.ManaPotion, amount = 1 });
	}

	public event EventHandler OnItemListChanged;

	public void AddItem(Item item)
	{
		if (item.IsStackable())
		{
			var itemAlreadyInInventory = false;
			foreach (var inventoryItem in itemList)
				if (inventoryItem.itemType == item.itemType)
				{
					inventoryItem.amount += item.amount;
					itemAlreadyInInventory = true;
				}
			if (!itemAlreadyInInventory) itemList.Add(item);
		}
		else { itemList.Add(item); }
		OnItemListChanged?.Invoke(this, EventArgs.Empty);
	}

	public void RemoveItemAmount(Item.ItemType itemType, int amount)
	{
		RemoveItem(new Item { itemType = itemType, amount = amount });
	}

	public void RemoveItem(Item item)
	{
		if (item.IsStackable())
		{
			Item itemInInventory = null;
			foreach (var inventoryItem in itemList)
				if (inventoryItem.itemType == item.itemType)
				{
					inventoryItem.amount -= item.amount;
					itemInInventory = inventoryItem;
				}
			if (itemInInventory != null && itemInInventory.amount <= 0)
				itemList.Remove(itemInInventory);
		}
		else { itemList.Remove(item); }
		OnItemListChanged?.Invoke(this, EventArgs.Empty);
	}

	public void UseItem(Item item)
	{
		useItemAction(item);
	}

	public List<Item> GetItemList()
	{
		return itemList;
	}
}
