#region Info
// -----------------------------------------------------------------------
// UI_Shop.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class UI_Shop : MonoBehaviour
{

	private Transform container;
	private IShopCustomer shopCustomer;
	private Transform shopItemTemplate;

	private void Awake()
	{
		container = transform.Find("container");
		shopItemTemplate = container.Find("shopItemTemplate");
		shopItemTemplate.gameObject.SetActive(false);
	}

	private void Start()
	{
		CreateItemButton(Item.ItemType.Armor_1,
				Item.GetSprite(Item.ItemType.Armor_1), "Armor 1",
				Item.GetCost(Item.ItemType.Armor_1), 0);
		CreateItemButton(Item.ItemType.Armor_2,
				Item.GetSprite(Item.ItemType.Armor_2), "Armor 2",
				Item.GetCost(Item.ItemType.Armor_2), 1);
		CreateItemButton(Item.ItemType.Helmet,
				Item.GetSprite(Item.ItemType.Helmet),
				"Helmet", Item.GetCost(Item.ItemType.Helmet), 2);
		CreateItemButton(Item.ItemType.Sword_2,
				Item.GetSprite(Item.ItemType.Sword_2), "Sword",
				Item.GetCost(Item.ItemType.Sword_2), 3);
		CreateItemButton(Item.ItemType.HealthPotion,
				Item.GetSprite(Item.ItemType.HealthPotion), "HealthPotion",
				Item.GetCost(Item.ItemType.HealthPotion), 4);

		Hide();
	}

	private void CreateItemButton(Item.ItemType itemType, Sprite itemSprite,
	                              string itemName, int itemCost,
	                              int positionIndex)
	{
		var shopItemTransform = Instantiate(shopItemTemplate, container);
		shopItemTransform.gameObject.SetActive(true);
		var shopItemRectTransform
				= shopItemTransform.GetComponent<RectTransform>();

		var shopItemHeight = 90f;
		shopItemRectTransform.anchoredPosition
				= new Vector2(0, -shopItemHeight * positionIndex);

		shopItemTransform.Find("nameText").GetComponent<TextMeshProUGUI>()
				.SetText(itemName);
		shopItemTransform.Find("costText").GetComponent<TextMeshProUGUI>()
				.SetText(itemCost.ToString());

		shopItemTransform.Find("itemImage").GetComponent<Image>().sprite
				= itemSprite;

		shopItemTransform.GetComponent<Button_UI>().ClickFunc = () =>
		{
			// Clicked on shop item button
			TryBuyItem(itemType);
		};
	}

	private void TryBuyItem(Item.ItemType itemType)
	{
		if (shopCustomer.TrySpendGoldAmount(
				Item.GetCost(itemType))) // Can afford cost
			shopCustomer.BoughtItem(itemType);
		else
			Tooltip_Warning.ShowTooltip_Static("Cannot afford " +
			                                   Item.GetCost(itemType) + "!");
	}

	public void Show(IShopCustomer shopCustomer)
	{
		this.shopCustomer = shopCustomer;
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
