#region Info
// -----------------------------------------------------------------------
// UI_CharacterEquipment.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion
public class UI_CharacterEquipment : MonoBehaviour
{

	[SerializeField] private Transform pfUI_Item;
	private UI_CharacterEquipmentSlot armorSlot;
	private CharacterEquipment characterEquipment;
	private UI_CharacterEquipmentSlot helmetSlot;

	private Transform itemContainer;
	private UI_CharacterEquipmentSlot weaponSlot;

	private void Awake()
	{
		itemContainer = transform.Find("itemContainer");
		weaponSlot = transform.Find("weaponSlot")
				.GetComponent<UI_CharacterEquipmentSlot>();
		helmetSlot = transform.Find("helmetSlot")
				.GetComponent<UI_CharacterEquipmentSlot>();
		armorSlot = transform.Find("armorSlot")
				.GetComponent<UI_CharacterEquipmentSlot>();

		weaponSlot.OnItemDropped += WeaponSlot_OnItemDropped;
		helmetSlot.OnItemDropped += HelmetSlot_OnItemDropped;
		armorSlot.OnItemDropped += ArmorSlot_OnItemDropped;
	}

	private void ArmorSlot_OnItemDropped(object sender,
	                                     UI_CharacterEquipmentSlot.
			                                     OnItemDroppedEventArgs e)
	{
		// Item dropped in Armor slot
		var equipSlot = CharacterEquipment.EquipSlot.Armor;
		if (characterEquipment.IsEquipSlotEmpty(equipSlot) &&
		    characterEquipment.CanEquipItem(equipSlot, e.item))
		{
			e.item.RemoveFromItemHolder();
			characterEquipment.EquipItem(e.item);
		}
		//characterEquipment.TryEquipItem(CharacterEquipment.EquipSlot.Armor, e.item);
	}

	private void HelmetSlot_OnItemDropped(object sender,
	                                      UI_CharacterEquipmentSlot.
			                                      OnItemDroppedEventArgs e)
	{
		// Item dropped in Helmet slot
		var equipSlot = CharacterEquipment.EquipSlot.Helmet;
		if (characterEquipment.IsEquipSlotEmpty(equipSlot) &&
		    characterEquipment.CanEquipItem(equipSlot, e.item))
		{
			e.item.RemoveFromItemHolder();
			characterEquipment.EquipItem(e.item);
		}
		//characterEquipment.TryEquipItem(CharacterEquipment.EquipSlot.Helmet, e.item);
	}

	private void WeaponSlot_OnItemDropped(object sender,
	                                      UI_CharacterEquipmentSlot.
			                                      OnItemDroppedEventArgs e)
	{
		// Item dropped in weapon slot
		var equipSlot = CharacterEquipment.EquipSlot.Weapon;
		if (characterEquipment.IsEquipSlotEmpty(equipSlot) &&
		    characterEquipment.CanEquipItem(equipSlot, e.item))
		{
			e.item.RemoveFromItemHolder();
			characterEquipment.EquipItem(e.item);
		}
		//characterEquipment.TryEquipItem(CharacterEquipment.EquipSlot.Weapon, e.item);
	}

	public void SetCharacterEquipment(CharacterEquipment characterEquipment)
	{
		this.characterEquipment = characterEquipment;
		UpdateVisual();

		characterEquipment.OnEquipmentChanged
				+= CharacterEquipment_OnEquipmentChanged;
	}

	private void
			CharacterEquipment_OnEquipmentChanged(object sender, EventArgs e)
	{
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		foreach (Transform child in itemContainer) Destroy(child.gameObject);

		var weaponItem = characterEquipment.GetWeaponItem();
		if (weaponItem != null)
		{
			var uiItemTransform = Instantiate(pfUI_Item, itemContainer);
			uiItemTransform.GetComponent<RectTransform>().anchoredPosition
					= weaponSlot.GetComponent<RectTransform>().anchoredPosition;
			uiItemTransform.localScale = Vector3.one * 1.5f;
			//uiItemTransform.GetComponent<CanvasGroup>().blocksRaycasts = false;
			var uiItem = uiItemTransform.GetComponent<UI_Item>();
			uiItem.SetItem(weaponItem);
			weaponSlot.transform.Find("emptyImage").gameObject.SetActive(false);
		}
		else
		{
			weaponSlot.transform.Find("emptyImage").gameObject.SetActive(true);
		}

		var armorItem = characterEquipment.GetArmorItem();
		if (armorItem != null)
		{
			var uiItemTransform = Instantiate(pfUI_Item, itemContainer);
			uiItemTransform.GetComponent<RectTransform>().anchoredPosition
					= armorSlot.GetComponent<RectTransform>().anchoredPosition;
			uiItemTransform.localScale = Vector3.one * 1.5f;
			//uiItemTransform.GetComponent<CanvasGroup>().blocksRaycasts = false;
			var uiItem = uiItemTransform.GetComponent<UI_Item>();
			uiItem.SetItem(armorItem);
			armorSlot.transform.Find("emptyImage").gameObject.SetActive(false);
		}
		else
		{
			armorSlot.transform.Find("emptyImage").gameObject.SetActive(true);
		}

		var helmetItem = characterEquipment.GetHelmetItem();
		if (helmetItem != null)
		{
			var uiItemTransform = Instantiate(pfUI_Item, itemContainer);
			uiItemTransform.GetComponent<RectTransform>().anchoredPosition
					= helmetSlot.GetComponent<RectTransform>().anchoredPosition;
			uiItemTransform.localScale = Vector3.one * 1.5f;
			//uiItemTransform.GetComponent<CanvasGroup>().blocksRaycasts = false;
			var uiItem = uiItemTransform.GetComponent<UI_Item>();
			uiItem.SetItem(helmetItem);
			helmetSlot.transform.Find("emptyImage").gameObject.SetActive(false);
		}
		else
		{
			helmetSlot.transform.Find("emptyImage").gameObject.SetActive(true);
		}
	}
}
