#region Info
// -----------------------------------------------------------------------
// ItemAssets.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class ItemAssets : MonoBehaviour
{


	public Transform pfItemWorld;

	public Sprite s_Sword;
	public Sprite s_HealthPotion;
	public Sprite s_ManaPotion;
	public Sprite s_Coin;
	public Sprite s_Medkit;
	public Sprite s_Sword_1;
	public Sprite s_Sword_2;
	public Sprite s_ArmorNone;
	public Sprite s_Armor_1;
	public Sprite s_Armor_2;
	public Sprite s_HelmetNone;
	public Sprite s_Helmet;
	public Sprite s_Wood;
	public Sprite s_Planks;
	public Sprite s_Stick;
	public Sprite s_Diamond;
	public Sprite s_Sword_Wood;
	public Sprite s_Sword_Diamond;

	public static ItemAssets Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}
}
