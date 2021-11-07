#region Info
// -----------------------------------------------------------------------
// Testing_WeaponAim.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class Testing_WeaponAim : MonoBehaviour
{

	[SerializeField] private PlayerAimWeapon playerAimWeapon;

	private void Start()
	{
		playerAimWeapon.OnShoot += PlayerAimWeapon_OnShoot;
	}

	private void PlayerAimWeapon_OnShoot(object sender,
	                                     PlayerAimWeapon.OnShootEventArgs e)
	{
		UtilsClass.ShakeCamera(.6f, .05f);
		WeaponTracer.Create(e.gunEndPointPosition, e.shootPosition);
		Shoot_Flash.AddFlash(e.gunEndPointPosition);

		var shootDir = (e.shootPosition - e.gunEndPointPosition).normalized;
		shootDir = UtilsClass.ApplyRotationToVector(shootDir, 90f);
		ShellParticleSystemHandler.Instance.SpawnShell(e.shellPosition,
				shootDir);
	}
}
