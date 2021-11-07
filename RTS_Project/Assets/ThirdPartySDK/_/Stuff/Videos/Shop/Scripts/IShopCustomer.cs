#region Info
// -----------------------------------------------------------------------
// IShopCustomer.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
public interface IShopCustomer
{

	void BoughtItem(Item.ItemType itemType);
	bool TrySpendGoldAmount(int goldAmount);
}
