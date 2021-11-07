#region Info
// -----------------------------------------------------------------------
// IAimShootAnims.cs
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
	public interface IAimShootAnims
	{

		event EventHandler<CharacterAim_Base.OnShootEventArgs> OnShoot;

		void SetAimTarget(Vector3 targetPosition);
		void ShootTarget(Vector3 targetPosition, Action onShootComplete);
	}
}
