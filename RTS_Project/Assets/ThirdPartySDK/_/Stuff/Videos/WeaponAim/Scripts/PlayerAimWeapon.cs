#region Info
// -----------------------------------------------------------------------
// PlayerAimWeapon.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class PlayerAimWeapon : MonoBehaviour
{
	private Animator aimAnimator;
	private Transform aimGunEndPointTransform;
	private Transform aimShellPositionTransform;
	private Transform aimTransform;

	private PlayerLookAt playerLookAt;

	private void Awake()
	{
		playerLookAt = GetComponent<PlayerLookAt>();
		aimTransform = transform.Find("Aim");
		aimAnimator = aimTransform.GetComponent<Animator>();
		aimGunEndPointTransform = aimTransform.Find("GunEndPointPosition");
		aimShellPositionTransform = aimTransform.Find("ShellPosition");
	}

	private void Update()
	{
		HandleAiming();
		HandleShooting();
	}

	public event EventHandler<OnShootEventArgs> OnShoot;

	private void HandleAiming()
	{
		var mousePosition = UtilsClass.GetMouseWorldPosition();

		var aimDirection = (mousePosition - aimTransform.position).normalized;
		var angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
		aimTransform.eulerAngles = new Vector3(0, 0, angle);

		var aimLocalScale = Vector3.one;
		if (angle > 90 || angle < -90)
			aimLocalScale.y = -1f;
		else
			aimLocalScale.y = +1f;
		aimTransform.localScale = aimLocalScale;

		playerLookAt.SetLookAtPosition(mousePosition);
	}

	private void HandleShooting()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var mousePosition = UtilsClass.GetMouseWorldPosition();

			aimAnimator.SetTrigger("Shoot");

			OnShoot?.Invoke(this, new OnShootEventArgs
			{
				gunEndPointPosition = aimGunEndPointTransform.position,
				shootPosition = mousePosition,
				shellPosition = aimShellPositionTransform.position
			});
		}
	}
	public class OnShootEventArgs : EventArgs
	{
		public Vector3 gunEndPointPosition;
		public Vector3 shellPosition;
		public Vector3 shootPosition;
	}
}
