#region Info
// -----------------------------------------------------------------------
// UI_Inventory.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class UI_Inventory : MonoBehaviour
{

	[SerializeField] private Transform pfUI_Item;

	private Inventory inventory;
	private Transform itemSlotContainer;
	private Transform itemSlotTemplate;
	private Player player;

	private void Awake()
	{
		itemSlotContainer = transform.Find("itemSlotContainer");
		itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
		itemSlotTemplate.gameObject.SetActive(false);
	}

	public void SetPlayer(Player player)
	{
		this.player = player;
	}

	public void SetInventory(Inventory inventory)
	{
		this.inventory = inventory;

		inventory.OnItemListChanged += Inventory_OnItemListChanged;

		RefreshInventoryItems();
	}

	private void Inventory_OnItemListChanged(object sender, EventArgs e)
	{
		RefreshInventoryItems();
	}

	private void RefreshInventoryItems()
	{
		foreach (Transform child in itemSlotContainer)
		{
			if (child == itemSlotTemplate) continue;
			Destroy(child.gameObject);
		}

		var x = 0;
		var y = 0;
		var itemSlotCellSize = 54f;
		foreach (var inventorySlot in inventory.GetInventorySlotArray())
		{
			var item = inventorySlot.GetItem();

			var itemSlotRectTransform
					= Instantiate(itemSlotTemplate, itemSlotContainer)
							.GetComponent<RectTransform>();
			itemSlotRectTransform.gameObject.SetActive(true);

			itemSlotRectTransform.GetComponent<Button_UI>().ClickFunc = () =>
			{
				// Use item
				//inventory.UseItem(item);
			};
			itemSlotRectTransform.GetComponent<Button_UI>().MouseRightClickFunc
					= ()
							=>
					{
						// Split item
						if (item.IsStackable()) // Is Stackable
							if (item.amount > 2)
							{
								// Can split
								var splitAmount
										= Mathf.FloorToInt(item.amount / 2f);
								item.amount -= splitAmount;
								var duplicateItem = new Item
								{
									itemType = item.itemType,
									amount = splitAmount
								};
								inventory.AddItem(duplicateItem);
							}

						// Drop item
						//Item duplicateItem = new Item { itemType = item.itemType, amount = item.amount };
						//inventory.RemoveItem(item);
						//ItemWorld.DropItem(player.GetPosition(), duplicateItem);
					};

			itemSlotRectTransform.anchoredPosition
					= new Vector2(x * itemSlotCellSize, -y * itemSlotCellSize);

			if (!inventorySlot.IsEmpty())
			{
				// Not Empty, has Item
				var uiItemTransform = Instantiate(pfUI_Item, itemSlotContainer);
				uiItemTransform.GetComponent<RectTransform>().anchoredPosition
						= itemSlotRectTransform.anchoredPosition;
				var uiItem = uiItemTransform.GetComponent<UI_Item>();
				uiItem.SetItem(item);
			}

			var tmpInventorySlot = inventorySlot;

			var uiItemSlot = itemSlotRectTransform.GetComponent<UI_ItemSlot>();
			uiItemSlot.SetOnDropAction(() =>
			{
				// Dropped on this UI Item Slot
				var draggedItem = UI_ItemDrag.Instance.GetItem();
				draggedItem.RemoveFromItemHolder();
				inventory.AddItem(draggedItem, tmpInventorySlot);
			});

			/*
			TextMeshProUGUI uiText = itemSlotRectTransform.Find("amountText").GetComponent<TextMeshProUGUI>();
			if (inventorySlot.IsEmpty()) {
			    // Empty
			    uiText.SetText("");
			} else {
			    if (item.amount > 1) {
			        uiText.SetText(item.amount.ToString());
			    } else {
			        uiText.SetText("");
			    }
			}
			*/

			x++;
			var itemRowMax = 7;
			if (x >= itemRowMax)
			{
				x = 0;
				y++;
			}
		}
	}
}
