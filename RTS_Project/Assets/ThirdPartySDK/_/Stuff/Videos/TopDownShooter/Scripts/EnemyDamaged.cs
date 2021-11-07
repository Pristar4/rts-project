#region Info
// -----------------------------------------------------------------------
// EnemyDamaged.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion

/*
 * Handles Enemy taking Damage
 * */
namespace TopDownShooter
{
	public class EnemyDamaged : MonoBehaviour
	{

		private EnemyMain enemyMain;

		private void Awake()
		{
			enemyMain = GetComponent<EnemyMain>();
		}

		private void Start()
		{
			enemyMain.OnDamaged += EnemyMain_OnDamaged;
		}

		private void EnemyMain_OnDamaged(object sender,
		                                 EnemyMain.OnDamagedEventArgs e)
		{
			var bloodDir
					= (GetPosition() - e.attacker.GetPosition()).normalized;
			BloodParticleSystemHandler.Instance.SpawnBlood(5, GetPosition(),
					bloodDir);

			var damageAmount
					= Mathf.RoundToInt(30 * e.damageMultiplier *
					                   Random.Range(.8f, 1.2f));
			DamagePopup.Create(GetPosition(), damageAmount, false);
			enemyMain.HealthSystem.Damage(damageAmount);

			//Sound_Manager.PlaySound(Sound_Manager.Sound.BodyHit_1, GetPosition());

			if (enemyMain.HealthSystem.IsDead())
			{
				FlyingBody.Create(GameAssets.i.pfEnemyFlyingBody, GetPosition(),
						bloodDir);
				enemyMain.DestroySelf();
				Sound_Manager.PlaySound(Sound_Manager.Sound.Minion_Dead,
						GetPosition());
			}
			else
			{
				// Knockback
				enemyMain.EnemyRigidbody2D.MovePosition(transform.position +
						bloodDir * 5f);
			}
		}

		private Vector3 GetPosition()
		{
			return enemyMain.GetPosition();
		}
	}

}
