#region Info
// -----------------------------------------------------------------------
// EnemySpawn.cs
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
 * Spawn this Enemy with a nice Dissolve Effect
 * */
namespace TopDownShooter
{
	public class EnemySpawn : MonoBehaviour
	{

		private EnemyMain enemyMain;

		private void Awake()
		{
			gameObject.SetActive(false);
			enemyMain = GetComponent<EnemyMain>();
		}

		private void Start()
		{
			enemyMain.HealthSystem.OnDead += HealthSystem_OnDead;
		}

		public event EventHandler OnDead;

		private void HealthSystem_OnDead(object sender, EventArgs e)
		{
			OnDead?.Invoke(this, EventArgs.Empty);
		}

		public void Spawn()
		{
			gameObject.SetActive(true);
			transform.SetParent(null); // Go to root

			var enemyPathfindingMovement
					= GetComponent<EnemyPathfindingMovement>();
			var enemyTargeting = GetComponent<EnemyTargeting>();

			if (enemyPathfindingMovement != null)
				enemyPathfindingMovement.enabled = false;
			if (enemyTargeting != null) enemyTargeting.enabled = false;

			FunctionTimer.Create(() =>
			{
				if (enemyPathfindingMovement != null)
					enemyPathfindingMovement.enabled = true;
				if (enemyTargeting != null) enemyTargeting.enabled = true;
			}, 1.5f);

			var dissolveAnimate = GetComponent<DissolveAnimate>();
			if (dissolveAnimate != null)
			{
				var dissolveTime = 2f;
				dissolveAnimate.StartDissolve(1f, -1f / dissolveTime);
			}
		}

		public bool IsAlive()
		{
			return !enemyMain.HealthSystem.IsDead();
		}

		public void KillEnemy()
		{
			enemyMain.Damage(Player.Instance, 1000f);
		}
	}
}
