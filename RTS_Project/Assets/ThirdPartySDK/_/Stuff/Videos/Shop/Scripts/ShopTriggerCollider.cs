#region Info
// -----------------------------------------------------------------------
// ShopTriggerCollider.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class ShopTriggerCollider : MonoBehaviour
{

	[SerializeField] private UI_Shop uiShop;

	private void OnTriggerEnter2D(Collider2D collider)
	{
		var shopCustomer = collider.GetComponent<IShopCustomer>();
		if (shopCustomer != null) uiShop.Show(shopCustomer);
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		var shopCustomer = collider.GetComponent<IShopCustomer>();
		if (shopCustomer != null) uiShop.Hide();
	}
}
