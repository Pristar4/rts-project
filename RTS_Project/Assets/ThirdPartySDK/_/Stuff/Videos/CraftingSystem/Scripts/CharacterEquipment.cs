#region Info
// -----------------------------------------------------------------------
// CharacterEquipment.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion
public class CharacterEquipment : MonoBehaviour, IItemHolder
{

	public enum EquipSlot
	{
		None,
		Helmet,
		Armor,
		Weapon
	}
	private Item armorItem;
	private Item helmetItem;

	private Player player;

	private Item weaponItem;

	private void Awake()
	{
		player = GetComponent<Player>();
	}

	public void RemoveItem(Item item)
	{
		if (GetWeaponItem() == item) SetWeaponItem(null);
		if (GetHelmetItem() == item) SetHelmetItem(null);
		if (GetArmorItem() == item) SetArmorItem(null);
	}

	public void AddItem(Item item)
	{
		EquipItem(item);
	}

	public bool CanAddItem()
	{
		return true;
	}

	public event EventHandler OnEquipmentChanged;

	public Item GetWeaponItem()
	{
		return weaponItem;
	}

	public Item GetHelmetItem()
	{
		return helmetItem;
	}

	public Item GetArmorItem()
	{
		return armorItem;
	}

	private void SetWeaponItem(Item weaponItem)
	{
		this.weaponItem = weaponItem;
		if (weaponItem != null)
		{
			weaponItem.SetItemHolder(this);
			player.SetEquipment(weaponItem.itemType);
		}
		else
		{
			// Unequipped weapon
			player.SetEquipment(Item.ItemType.SwordNone);
		}
		OnEquipmentChanged?.Invoke(this, EventArgs.Empty);
	}

	private void SetHelmetItem(Item helmetItem)
	{
		this.helmetItem = helmetItem;
		if (helmetItem != null)
		{
			helmetItem.SetItemHolder(this);
			player.SetEquipment(helmetItem.itemType);
		}
		else
		{
			// Unequipped Helmet
			player.SetEquipment(Item.ItemType.HelmetNone);
		}
		OnEquipmentChanged?.Invoke(this, EventArgs.Empty);
	}

	private void SetArmorItem(Item armorItem)
	{
		this.armorItem = armorItem;
		if (armorItem != null)
		{
			armorItem.SetItemHolder(this);
			player.SetEquipment(armorItem.itemType);
		}
		else
		{
			// Unequipped Armor
			player.SetEquipment(Item.ItemType.ArmorNone);
		}
		OnEquipmentChanged?.Invoke(this, EventArgs.Empty);
	}

	public void EquipItem(Item item)
	{
		switch (item.GetEquipSlot())
		{
			default:
			case EquipSlot.Armor:
				SetArmorItem(item);
				break;
			case EquipSlot.Helmet:
				SetHelmetItem(item);
				break;
			case EquipSlot.Weapon:
				SetWeaponItem(item);
				break;
		}
	}

	public void TryEquipItem(EquipSlot equipSlot, Item item)
	{
		if (CanEquipItem(equipSlot, item)) EquipItem(item);
	}

	public Item GetEquippedItem(EquipSlot equipSlot)
	{
		switch (equipSlot)
		{
			default:
			case EquipSlot.Armor: return GetArmorItem();
			case EquipSlot.Helmet: return GetHelmetItem();
			case EquipSlot.Weapon: return GetWeaponItem();
		}
	}

	public bool IsEquipSlotEmpty(EquipSlot equipSlot)
	{
		return GetEquippedItem(equipSlot) == null; // Nothing currently equipped
	}

	public bool CanEquipItem(EquipSlot equipSlot, Item item)
	{
		return equipSlot == item.GetEquipSlot(); // Item matches this EquipSlot
	}
}
