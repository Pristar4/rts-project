#region Info
// -----------------------------------------------------------------------
// Tower.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class Tower : MonoBehaviour
{
	private int damageAmount;

	private Vector3 projectileShootFromPosition;
	private float range;
	private float shootTimer;
	private float shootTimerMax;

	private void Awake()
	{
		projectileShootFromPosition
				= transform.Find("ProjectileShootFromPosition").position;
		range = 60f;
		damageAmount = 25;
		shootTimerMax = .4f;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			//CMDebug.TextPopupMouse("Click!");
			//ProjectileArrow.Create(projectileShootFromPosition, UtilsClass.GetMouseWorldPosition());
		}

		shootTimer -= Time.deltaTime;

		if (shootTimer <= 0f)
		{
			shootTimer = shootTimerMax;

			var enemy = GetClosestEnemy();
			if (enemy != null) // Enemy in range!
				ProjectileArrow.Create(projectileShootFromPosition, enemy,
						Random.Range(damageAmount - 5, damageAmount + 5));
		}
	}

	private void OnMouseEnter()
	{
		UpgradeOverlay.Show_Static(this);
	}

	private Enemy GetClosestEnemy()
	{
		return Enemy.GetClosestEnemy(transform.position, range);
	}

	public float GetRange()
	{
		return range;
	}

	public void UpgradeRange()
	{
		range += 10f;
	}

	public void UpgradeDamageAmount()
	{
		damageAmount += 5;
	}
}
