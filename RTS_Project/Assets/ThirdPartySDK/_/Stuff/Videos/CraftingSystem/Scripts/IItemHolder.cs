#region Info
// -----------------------------------------------------------------------
// IItemHolder.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
public interface IItemHolder
{

	void RemoveItem(Item item);
	void AddItem(Item item);
	bool CanAddItem();
}
