#region Info
// -----------------------------------------------------------------------
// Window_Items.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class Window_Items : MonoBehaviour
{

	private void Start()
	{
		for (var i = 1; i <= 6; i++)
		{
			var itemInfo = transform.Find("itemBtn_" + i)
					.GetComponent<ItemInfo>();
			//Tooltip_ItemStats.AddTooltip(transform.Find("itemBtn_" + i), itemInfo.sprite, itemInfo.itemName, itemInfo.itemDescription, itemInfo.DEX, itemInfo.CON, itemInfo.STR);
		}
	}
}
