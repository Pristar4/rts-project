#region Info
// -----------------------------------------------------------------------
// CraftingSystem.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
#endregion
public class CraftingSystem : IItemHolder
{

	public const int GRID_SIZE = 3;

	private readonly Item[,] itemArray;

	private readonly Dictionary<Item.ItemType, Item.ItemType[,]>
			recipeDictionary;
	private Item outputItem;

	public CraftingSystem()
	{
		itemArray = new Item[GRID_SIZE, GRID_SIZE];

		recipeDictionary = new Dictionary<Item.ItemType, Item.ItemType[,]>();

		// Stick
		var recipe = new Item.ItemType[GRID_SIZE, GRID_SIZE];
		recipe[0, 2] = Item.ItemType.None;
		recipe[1, 2] = Item.ItemType.None;
		recipe[2, 2] = Item.ItemType.None;
		recipe[0, 1] = Item.ItemType.None;
		recipe[1, 1] = Item.ItemType.Wood;
		recipe[2, 1] = Item.ItemType.None;
		recipe[0, 0] = Item.ItemType.None;
		recipe[1, 0] = Item.ItemType.Wood;
		recipe[2, 0] = Item.ItemType.None;
		recipeDictionary[Item.ItemType.Stick] = recipe;

		// Wooden Sword
		recipe = new Item.ItemType[GRID_SIZE, GRID_SIZE];
		recipe[0, 2] = Item.ItemType.None;
		recipe[1, 2] = Item.ItemType.Wood;
		recipe[2, 2] = Item.ItemType.None;
		recipe[0, 1] = Item.ItemType.None;
		recipe[1, 1] = Item.ItemType.Wood;
		recipe[2, 1] = Item.ItemType.None;
		recipe[0, 0] = Item.ItemType.None;
		recipe[1, 0] = Item.ItemType.Stick;
		recipe[2, 0] = Item.ItemType.None;
		recipeDictionary[Item.ItemType.Sword_Wood] = recipe;

		// Diamond Sword
		recipe = new Item.ItemType[GRID_SIZE, GRID_SIZE];
		recipe[0, 2] = Item.ItemType.None;
		recipe[1, 2] = Item.ItemType.Diamond;
		recipe[2, 2] = Item.ItemType.None;
		recipe[0, 1] = Item.ItemType.None;
		recipe[1, 1] = Item.ItemType.Diamond;
		recipe[2, 1] = Item.ItemType.None;
		recipe[0, 0] = Item.ItemType.None;
		recipe[1, 0] = Item.ItemType.Stick;
		recipe[2, 0] = Item.ItemType.None;
		recipeDictionary[Item.ItemType.Sword_Diamond] = recipe;
	}

	public void RemoveItem(Item item)
	{
		if (item == outputItem)
		{
			// Removed output item
			ConsumeRecipeItems();
			CreateOutput();
			OnGridChanged?.Invoke(this, EventArgs.Empty);
		}
		else
		{
			// Removed item from grid
			for (var x = 0; x < GRID_SIZE; x++)
			for (var y = 0; y < GRID_SIZE; y++)
				if (GetItem(x, y) == item) // Removed this one
					RemoveItem(x, y);
		}
	}

	public void AddItem(Item item) { }

	public bool CanAddItem() { return false; }

	public event EventHandler OnGridChanged;

	public bool IsEmpty(int x, int y)
	{
		return itemArray[x, y] == null;
	}

	public Item GetItem(int x, int y)
	{
		return itemArray[x, y];
	}

	public void SetItem(Item item, int x, int y)
	{
		if (item != null)
		{
			item.RemoveFromItemHolder();
			item.SetItemHolder(this);
		}
		itemArray[x, y] = item;
		CreateOutput();
		OnGridChanged?.Invoke(this, EventArgs.Empty);
	}

	public void IncreaseItemAmount(int x, int y)
	{
		GetItem(x, y).amount++;
		OnGridChanged?.Invoke(this, EventArgs.Empty);
	}

	public void DecreaseItemAmount(int x, int y)
	{
		if (GetItem(x, y) != null)
		{
			GetItem(x, y).amount--;
			if (GetItem(x, y).amount == 0) RemoveItem(x, y);
			OnGridChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public void RemoveItem(int x, int y)
	{
		SetItem(null, x, y);
	}

	public bool TryAddItem(Item item, int x, int y)
	{
		if (IsEmpty(x, y))
		{
			SetItem(item, x, y);
			return true;
		}
		if (item.itemType == GetItem(x, y).itemType)
		{
			IncreaseItemAmount(x, y);
			return true;
		}
		return false;
	}


	private Item.ItemType GetRecipeOutput()
	{
		foreach (var recipeItemType in recipeDictionary.Keys)
		{
			var recipe = recipeDictionary[recipeItemType];

			var completeRecipe = true;
			for (var x = 0; x < GRID_SIZE; x++)
			for (var y = 0; y < GRID_SIZE; y++)
				if (recipe[x, y] !=
				    Item.ItemType.None) // Recipe has Item in this position
					if (IsEmpty(x, y) ||
					    GetItem(x, y).itemType !=
					    recipe[x, y]) // Empty position or different itemType
						completeRecipe = false;

			if (completeRecipe) return recipeItemType;
		}
		return Item.ItemType.None;
	}

	private void CreateOutput()
	{
		var recipeOutput = GetRecipeOutput();
		if (recipeOutput == Item.ItemType.None) { outputItem = null; }
		else
		{
			outputItem = new Item { itemType = recipeOutput };
			outputItem.SetItemHolder(this);
		}
	}

	public Item GetOutputItem()
	{
		return outputItem;
	}

	public void ConsumeRecipeItems()
	{
		for (var x = 0; x < GRID_SIZE; x++)
		for (var y = 0; y < GRID_SIZE; y++)
			DecreaseItemAmount(x, y);
	}
}
