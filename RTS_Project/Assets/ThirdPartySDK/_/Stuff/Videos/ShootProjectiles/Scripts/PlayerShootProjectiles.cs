#region Info
// -----------------------------------------------------------------------
// PlayerShootProjectiles.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class PlayerShootProjectiles : MonoBehaviour
{

	[SerializeField] private Transform pfBullet;
	[SerializeField] private Transform pfBulletPhysics;
	private ShootActionDelegate shootAction;

	private void Awake()
	{
		shootAction = ShootPhysics;
		GetComponent<CharacterAim_Base>().OnShoot
				+= PlayerShootProjectiles_OnShoot;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T)) shootAction = ShootTransform;
		if (Input.GetKeyDown(KeyCode.Y)) shootAction = ShootPhysics;
		if (Input.GetKeyDown(KeyCode.U)) shootAction = ShootRaycast;
	}

	private void PlayerShootProjectiles_OnShoot(object sender,
	                                            CharacterAim_Base.
			                                            OnShootEventArgs
			                                            e)
	{
		// Shoot
		shootAction(e.gunEndPointPosition, e.shootPosition);
	}

	private void ShootRaycast(Vector3 gunEndPointPosition,
	                          Vector3 shootPosition)
	{
		var shootDir = (shootPosition - gunEndPointPosition).normalized;
		BulletRaycast.Shoot(gunEndPointPosition, shootDir);

		WeaponTracer.Create(gunEndPointPosition,
				UtilsClass.GetMouseWorldPosition());
	}

	private void ShootPhysics(Vector3 gunEndPointPosition,
	                          Vector3 shootPosition)
	{
		var bulletTransform = Instantiate(pfBulletPhysics, gunEndPointPosition,
				Quaternion.identity);

		var shootDir = (shootPosition - gunEndPointPosition).normalized;
		bulletTransform.GetComponent<BulletPhysics>().Setup(shootDir);
	}

	private void ShootTransform(Vector3 gunEndPointPosition,
	                            Vector3 shootPosition)
	{
		var bulletTransform
				= Instantiate(pfBullet, gunEndPointPosition,
						Quaternion.identity);

		var shootDir = (shootPosition - gunEndPointPosition).normalized;
		bulletTransform.GetComponent<Bullet>().Setup(shootDir);
	}

	private delegate void ShootActionDelegate(Vector3 gunEndPointPosition,
	                                          Vector3 shootPosition);
}
