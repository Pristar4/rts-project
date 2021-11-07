#region Info
// -----------------------------------------------------------------------
// EnemyTurretAimShootAnims.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
#endregion

/*
 * Handles Aim and Shoot visuals
 * */
namespace TopDownShooter
{
	public class EnemyTurretAimShootAnims : MonoBehaviour, IAimShootAnims
	{
		private Transform turretAimGunEndPointTransform;

		private Transform turretAimTransform;
		private Animator turretAnimator;

		private void Awake()
		{
			turretAimTransform = transform.Find("TurretAim");
			turretAimGunEndPointTransform
					= turretAimTransform.Find("GunEndPointTransform");
			turretAnimator = GetComponent<Animator>();
		}

		public event EventHandler<CharacterAim_Base.OnShootEventArgs> OnShoot;

		public void SetAimTarget(Vector3 targetPosition)
		{
			var aimDir = (targetPosition - transform.position).normalized;
			turretAimTransform.eulerAngles = new Vector3(0, 0,
					UtilsClass.GetAngleFromVectorFloat(aimDir));
		}

		public void ShootTarget(Vector3 targetPosition, Action onShootComplete)
		{
			SetAimTarget(targetPosition);

			// Check for hits
			var gunEndPointPosition = turretAimGunEndPointTransform.position;
			//int layerMask = ~(1 << GameAssets.i.enemyLayer | 1 << GameAssets.i.ignoreRaycastLayer | 1 << GameAssets.i.shieldLayer);
			var layerMask = ~0;
			var raycastHit = Physics2D.Raycast(gunEndPointPosition,
					(targetPosition - gunEndPointPosition).normalized,
					Vector3.Distance(gunEndPointPosition, targetPosition),
					layerMask);
			GameObject hitObject = null;
			if (raycastHit.collider != null)
			{
				// Hit something
				targetPosition = (Vector3)raycastHit.point +
				                 (targetPosition - gunEndPointPosition)
				                 .normalized;
				hitObject = raycastHit.collider.gameObject;
			}

			turretAnimator.SetTrigger("Shoot");

			OnShoot?.Invoke(this, new CharacterAim_Base.OnShootEventArgs
			{
				gunEndPointPosition = turretAimGunEndPointTransform.position,
				hitObject = hitObject,
				shootPosition = targetPosition
			});
			onShootComplete();
		}
	}

}
