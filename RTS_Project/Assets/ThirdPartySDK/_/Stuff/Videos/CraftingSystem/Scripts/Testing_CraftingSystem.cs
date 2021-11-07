#region Info
// -----------------------------------------------------------------------
// Testing_CraftingSystem.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class Testing_CraftingSystem : MonoBehaviour
{

	[SerializeField] private Player player;
	[SerializeField] private UI_Inventory uiInventory;

	[SerializeField] private UI_CharacterEquipment uiCharacterEquipment;
	[SerializeField] private CharacterEquipment characterEquipment;

	[SerializeField] private UI_CraftingSystem uiCraftingSystem;

	private void Start()
	{
		uiInventory.SetPlayer(player);
		uiInventory.SetInventory(player.GetInventory());

		uiCharacterEquipment.SetCharacterEquipment(characterEquipment);

		var craftingSystem = new CraftingSystem();
		//Item item = new Item { itemType = Item.ItemType.Diamond, amount = 1 };
		//craftingSystem.SetItem(item, 0, 0);
		//Debug.Log(craftingSystem.GetItem(0, 0));

		uiCraftingSystem.SetCraftingSystem(craftingSystem);
	}
}
