#region Info
// -----------------------------------------------------------------------
// UI_WeaponBar.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

namespace TopDownShooter
{
	public class UI_WeaponBar : MonoBehaviour
	{

		private void Start()
		{
			Player.Instance.OnPickedUpWeapon += Instance_OnPickedUpWeapon;
		}

		private void Instance_OnPickedUpWeapon(object sender, EventArgs e)
		{
			var weaponType = (Weapon.WeaponType)sender;
			switch (weaponType)
			{
				case Weapon.WeaponType.Shotgun:
					transform.Find("shotgun").GetComponent<CanvasGroup>().alpha
							= 1f;
					break;
				case Weapon.WeaponType.Rifle:
					transform.Find("rifle").GetComponent<CanvasGroup>().alpha
							= 1f;
					break;
			}
		}
	}
}
