#region Info
// -----------------------------------------------------------------------
// Weapon.cs
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
	public class Weapon
	{

		public enum WeaponType
		{
			Pistol,
			Shotgun,
			Rifle,
			Sword,
			Punch
		}

		private readonly WeaponType weaponType;
		private int ammo;

		public Weapon(WeaponType weaponType)
		{
			this.weaponType = weaponType;
			ammo = GetAmmoMax();
		}

		public event EventHandler OnAmmoChanged;

		public WeaponType GetWeaponType()
		{
			return weaponType;
		}

		public int GetAmmo()
		{
			return ammo;
		}

		public bool TrySpendAmmo()
		{
			if (ammo > 0)
			{
				ammo--;
				OnAmmoChanged?.Invoke(this, EventArgs.Empty);
				return true;
			}
			return false;
		}

		public void Reload()
		{
			ammo = GetAmmoMax();
			OnAmmoChanged?.Invoke(this, EventArgs.Empty);
		}

		public bool CanReload()
		{
			return ammo < GetAmmoMax();
		}

		public int GetAmmoMax()
		{
			switch (weaponType)
			{
				default:
					return 99;
				case WeaponType.Pistol:
					return 12;
				case WeaponType.Shotgun:
					return 4;
				case WeaponType.Rifle:
					return 25;
			}
		}

		public float GetDamageMultiplier()
		{
			switch (weaponType)
			{
				default:
				case WeaponType.Pistol: return 1.2f;
				case WeaponType.Shotgun: return 1.9f;
				case WeaponType.Rifle:   return 0.6f;
			}
		}

		public float GetFireRate()
		{
			switch (weaponType)
			{
				default:
				case WeaponType.Pistol: return .15f;
				case WeaponType.Shotgun: return .20f;
				case WeaponType.Rifle:   return .09f;
			}
		}

		public Sprite GetSprite()
		{
			switch (weaponType)
			{
				default:
					return null;
				//case WeaponType.Pistol: return GameAssets.i.s_PistolIcon;
				//case WeaponType.Shotgun: return GameAssets.i.s_ShotgunIcon;
				//case WeaponType.Rifle: return GameAssets.i.s_RifleIcon;
			}
		}
	}

}
