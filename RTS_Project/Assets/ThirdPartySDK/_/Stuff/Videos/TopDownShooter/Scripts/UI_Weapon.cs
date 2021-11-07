#region Info
// -----------------------------------------------------------------------
// UI_Weapon.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace TopDownShooter
{
	public class UI_Weapon : MonoBehaviour
	{
		private TextMeshProUGUI ammoText;
		private Weapon weapon;

		private Image weaponImage;

		private void Awake()
		{
			weaponImage = transform.Find("weaponImage").GetComponent<Image>();
			ammoText = transform.Find("ammoText")
					.GetComponent<TextMeshProUGUI>();
		}

		public void SetWeapon(Weapon weapon)
		{
			this.weapon = weapon;

			weaponImage.sprite = weapon.GetSprite();
			weapon.OnAmmoChanged += Weapon_OnAmmoChanged;
			UpdateAmmoText();
		}

		private void Weapon_OnAmmoChanged(object sender, EventArgs e)
		{
			UpdateAmmoText();
		}

		private void UpdateAmmoText()
		{
			ammoText.text = weapon.GetAmmo() + "/" + weapon.GetAmmoMax();
			if (weapon.GetAmmo() == 0)
				ammoText.color = Color.red;
			else
				ammoText.color = Color.white;
		}
	}
}
