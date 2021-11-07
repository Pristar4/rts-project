#region Info
// -----------------------------------------------------------------------
// BulletRaycast.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public static class BulletRaycast
{

	public static void Shoot(Vector3 shootPosition, Vector3 shootDirection)
	{
		var raycastHit2D = Physics2D.Raycast(shootPosition, shootDirection);

		if (raycastHit2D.collider != null)
		{
			// Hit!
			var target = raycastHit2D.collider.GetComponent<Target>();
			if (target != null) target.Damage();
		}
	}
}
