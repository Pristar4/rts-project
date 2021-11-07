#region Info
// -----------------------------------------------------------------------
// Enemy.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion

/*
 * Handles General Enemy Spawning and Visuals
 * */
namespace TopDownShooter
{
	public class Enemy : MonoBehaviour
	{


		public enum EnemyType
		{
			Minion,
			Archer,
			Charger
		}

		public static List<Enemy> enemyList = new List<Enemy>();
		private World_Bar healthBar;

		private State state;

		public EnemyMain EnemyMain { get; private set; }

		private void Awake()
		{
			enemyList.Add(this);
			EnemyMain = GetComponent<EnemyMain>();
			state = State.Normal;
			healthBar = new World_Bar(transform, new Vector3(0, 10),
					new Vector3(10, 1.3f), Color.grey, Color.red, 1f, 10000,
					new World_Bar.Outline { color = Color.black, size = .5f });
		}

		private void Start()
		{
			if (EnemyMain.AimShootAnims != null)
				EnemyMain.AimShootAnims.OnShoot += EnemyAim_OnShoot;
			EnemyMain.OnDestroySelf += EnemyMain_OnDestroySelf;
			EnemyMain.HealthSystem.OnHealthChanged
					+= HealthSystem_OnHealthChanged;
			EnemyMain.HealthSystem.OnHealthMaxChanged
					+= HealthSystem_OnHealthMaxChanged;
			healthBar.SetLocalScale(
					new Vector3(EnemyMain.HealthSystem.GetHealthMax() / 12f,
							1.5f));
		}

		public static Enemy GetClosestEnemy(Vector3 position, float maxRange)
		{
			Enemy closest = null;
			foreach (var enemy in enemyList)
			{
				if (enemy.IsDead()) continue;
				if (Vector3.Distance(position, enemy.GetPosition()) <= maxRange)
				{
					if (closest == null) { closest = enemy; }
					else
					{
						if (Vector3.Distance(position, enemy.GetPosition()) <
						    Vector3.Distance(position, closest.GetPosition()))
							closest = enemy;
					}
				}
			}
			return closest;
		}

		public static Enemy Create(Vector3 position, EnemyType enemyType)
		{
			var enemyPrefab = GameAssets.i.pfEnemy;

			switch (enemyType)
			{
				default:
				case EnemyType.Minion:
					enemyPrefab = GameAssets.i.pfEnemy;
					break;
				//case EnemyType.Archer: enemyPrefab = GameAssets.i.pfEnemyArcher; break;
				//case EnemyType.Charger: enemyPrefab = GameAssets.i.pfEnemyCharger; break;
			}

			var enemyTransform
					= Instantiate(enemyPrefab, position, Quaternion.identity);

			var enemyHandler = enemyTransform.GetComponent<Enemy>();

			return enemyHandler;
		}

		private void HealthSystem_OnHealthMaxChanged(object sender, EventArgs e)
		{
			healthBar.SetLocalScale(
					new Vector3(EnemyMain.HealthSystem.GetHealthMax() / 12f,
							1.5f));
		}

		private void EnemyMain_OnDestroySelf(object sender, EventArgs e)
		{
			Destroy(gameObject);
		}

		private void EnemyAim_OnShoot(object sender,
		                              CharacterAim_Base.OnShootEventArgs e)
		{
			Shoot_Flash.AddFlash(e.gunEndPointPosition);
			WeaponTracer.Create(e.gunEndPointPosition, e.shootPosition);
			UtilsClass.ShakeCamera(.3f, .05f);
			SpawnBulletShellCasing(e.gunEndPointPosition, e.shootPosition);

			// Player hit?
			if (e.hitObject != null)
			{
				var player = e.hitObject.GetComponent<Player>();
				if (player != null)
					player.Damage(this, EnemyMain.EnemyStats.damageMultiplier);
			}
		}

		private void SpawnBulletShellCasing(Vector3 gunEndPointPosition,
		                                    Vector3 shootPosition)
		{
			var shellSpawnPosition = gunEndPointPosition;
			var shootDir = (shootPosition - gunEndPointPosition).normalized;
			var backOffsetPosition = 8f;

			shellSpawnPosition += shootDir * -1f * backOffsetPosition;

			var applyRotation = Random.Range(+130f, +95f);
			if (shootDir.x < 0) applyRotation *= -1f;

			var shellMoveDir
					= UtilsClass.ApplyRotationToVector(shootDir, applyRotation);

			ShellParticleSystemHandler.Instance.SpawnShell(shellSpawnPosition,
					shellMoveDir);
		}

		private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
		{
			healthBar.SetSize(EnemyMain.HealthSystem.GetHealthNormalized());
		}

		public Vector3 GetPosition()
		{
			return transform.position;
		}

		public bool IsDead()
		{
			return EnemyMain.HealthSystem.IsDead();
		}

		public void Damage(Player attacker, float damageMultiplier)
		{
			EnemyMain.Damage(attacker, damageMultiplier);
		}

		public interface IEnemyTargetable
		{
			GameObject GetGameObject();
			Vector3 GetPosition();
			void Damage(Enemy attacker, float damageMultiplier);
		}

		private enum State
		{
			Normal
		}
	}

}
