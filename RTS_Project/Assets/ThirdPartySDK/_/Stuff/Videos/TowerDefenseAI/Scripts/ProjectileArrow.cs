#region Info
// -----------------------------------------------------------------------
// ProjectileArrow.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class ProjectileArrow : MonoBehaviour
{
	private int damageAmount;

	private Enemy enemy;

	private void Update()
	{
		if (enemy == null || enemy.IsDead())
		{
			// Enemy already dead
			Destroy(gameObject);
			return;
		}

		var targetPosition = enemy.GetPosition();
		var moveDir = (targetPosition - transform.position).normalized;

		var moveSpeed = 130f;

		transform.position += moveDir * moveSpeed * Time.deltaTime;

		var angle = UtilsClass.GetAngleFromVectorFloat(moveDir);
		transform.eulerAngles = new Vector3(0, 0, angle);

		var destroySelfDistance = 1f;
		if (Vector3.Distance(transform.position, targetPosition) <
		    destroySelfDistance) //enemy.Damage(damageAmount);
			Destroy(gameObject);
	}

	public static void Create(Vector3 spawnPosition, Enemy enemy,
	                          int damageAmount)
	{
		var arrowTransform = Instantiate(GameAssets.i.pfProjectileArrow,
				spawnPosition, Quaternion.identity);

		var projectileArrow = arrowTransform.GetComponent<ProjectileArrow>();
		projectileArrow.Setup(enemy, damageAmount);
	}

	private void Setup(Enemy enemy, int damageAmount)
	{
		this.enemy = enemy;
		this.damageAmount = damageAmount;
	}
}
