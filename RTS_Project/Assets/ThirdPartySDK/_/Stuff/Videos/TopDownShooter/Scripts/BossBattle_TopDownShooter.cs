#region Info
// -----------------------------------------------------------------------
// BossBattle_TopDownShooter.cs
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

namespace TopDownShooter
{
	public class BossBattle_TopDownShooter : MonoBehaviour
	{

		[SerializeField] private Enemy enemyTurret;
		[SerializeField] private GameObject keyGameObject;
		[SerializeField] private DoorAnims entryDoorAnims;
		[SerializeField] private CaptureOnTriggerEnter2D startBattleTrigger;
		private List<Enemy> spawnedEnemyList;
		private List<Vector3> spawnPositionList;

		private Stage stage;
		private GameObject stage_1;
		private GameObject stage_2;
		private GameObject stage_3;

		private void Awake()
		{
			stage_1 = transform.Find("Stage_1").gameObject;
			stage_2 = transform.Find("Stage_2").gameObject;
			stage_3 = transform.Find("Stage_3").gameObject;

			stage_1.SetActive(false);
			stage_2.SetActive(false);
			stage_3.SetActive(false);

			stage = Stage._0;

			keyGameObject.SetActive(false);

			spawnPositionList = new List<Vector3>();
			foreach (Transform spawnPositionTransform in
					transform.Find("SpawnPositions"))
				spawnPositionList.Add(spawnPositionTransform.position);

			spawnedEnemyList = new List<Enemy>();
		}

		private void Start()
		{
			enemyTurret.EnemyMain.HealthSystem.OnDamaged += Turret_OnDamaged;
			enemyTurret.EnemyMain.HealthSystem.OnDead += Turret_OnDead;
			// Disable Turret Targeting
			enemyTurret.GetComponent<EnemyTargeting>().enabled = false;

			startBattleTrigger.OnPlayerTriggerEnter2D
					+= StartBattleTrigger_OnPlayerTriggerEnter2D;
		}

		private void Turret_OnDead(object sender, EventArgs e)
		{
			Debug.Log("Boss dead!");
			FunctionPeriodic.StopAllFunc("BossSpawnEnemy");
			FunctionPeriodic.StopAllFunc("BossSpawnEnemy_2");
			FunctionPeriodic.StopAllFunc("BossSpawnHealthPickup");
			DestroyAllEnemies();

			keyGameObject.SetActive(true);
		}

		private void StartBattleTrigger_OnPlayerTriggerEnter2D(
				object sender, EventArgs e)
		{
			StartBattle();
			startBattleTrigger.OnPlayerTriggerEnter2D
					-= StartBattleTrigger_OnPlayerTriggerEnter2D;
		}

		private void Turret_OnDamaged(object sender, EventArgs e)
		{
			var healthNormalized
					= enemyTurret.EnemyMain.HealthSystem.GetHealthNormalized();

			switch (stage)
			{
				case Stage._0:
					if (healthNormalized < .7f) StartNextStage();
					break;
				case Stage._1:
					if (healthNormalized < .5f) StartNextStage();
					break;
				case Stage._2:
					if (healthNormalized < .25f) StartNextStage();
					break;
			}
		}

		private void StartBattle()
		{
			// Enable Turret Targeting
			enemyTurret.GetComponent<EnemyTargeting>().enabled = true;

			SpawnEnemy();
			SpawnEnemy();

			entryDoorAnims.SetColor(DoorAnims.ColorName.Red);
			entryDoorAnims.CloseDoor();
		}

		private void StartNextStage()
		{
			switch (stage)
			{
				case Stage._0:
					stage = Stage._1;
					stage_1.SetActive(true);
					FunctionTimer.Create(SpawnEnemy, 1f);
					FunctionTimer.Create(SpawnEnemy, 3f);
					SpawnEnemy();
					SpawnEnemy();
					SpawnPickup();
					break;
				case Stage._1:
					stage = Stage._2;
					stage_2.SetActive(true);
					FunctionPeriodic.Create(SpawnEnemy, 4f, "BossSpawnEnemy");
					FunctionTimer.Create(SpawnHealthPickup, 4f);
					FunctionTimer.Create(SpawnEnemy, 0.1f);
					FunctionTimer.Create(SpawnEnemy, 0.5f);
					FunctionTimer.Create(SpawnEnemy, 5.0f);
					FunctionTimer.Create(SpawnEnemy, 8.0f);
					FunctionPeriodic.Create(SpawnPickup, 5f,
							"BossSpawnHealthPickup");
					SpawnEnemy();
					SpawnPickup();
					break;
				case Stage._2:
					stage = Stage._3;
					stage_2.SetActive(false);
					stage_3.SetActive(true);
					FunctionPeriodic.Create(SpawnEnemy, 6f, "BossSpawnEnemy_2");
					FunctionTimer.Create(SpawnEnemy, 0.1f);
					FunctionTimer.Create(SpawnEnemy, 0.5f);
					FunctionTimer.Create(SpawnEnemy, 1.0f);
					FunctionTimer.Create(SpawnEnemy, 1.5f);

					FunctionTimer.Create(SpawnEnemy, 5.0f);

					FunctionTimer.Create(SpawnEnemy, 8.5f);
					FunctionTimer.Create(SpawnEnemy, 10.0f);
					FunctionTimer.Create(SpawnEnemy, 11.0f);
					FunctionTimer.Create(SpawnEnemy, 12.0f);
					SpawnPickup();
					SpawnPickup();
					break;
			}
		}

		private void SpawnEnemy()
		{
			var maxEnemyCount = 6;
			var aliveCount = 0;
			foreach (var spawnedEnemy in spawnedEnemyList)
				if (!spawnedEnemy.IsDead())
					aliveCount++;
			if (aliveCount >= maxEnemyCount) return; // Too many enemies!

			var position
					= spawnPositionList[
							Random.Range(0, spawnPositionList.Count)];

			Enemy.EnemyType enemyType;
			var rnd = Random.Range(0, 100);

			enemyType = Enemy.EnemyType.Archer;
			if (rnd < 65) enemyType = Enemy.EnemyType.Charger;
			if (rnd < 15) enemyType = Enemy.EnemyType.Minion;

			var enemy = Enemy.Create(position, enemyType);
			enemy.GetComponent<EnemySpawn>().Spawn();

			spawnedEnemyList.Add(enemy);
		}

		private void SpawnPickup()
		{
			SpawnHealthPickup();
		}

		private void SpawnHealthPickup()
		{
			var position
					= spawnPositionList[
							Random.Range(0, spawnPositionList.Count)];
			//Instantiate(GameAssets.i.pfPickupHealth, position, Quaternion.identity);
			Instantiate(null, position, Quaternion.identity);
		}

		private void DestroyAllEnemies()
		{
			foreach (var enemy in spawnedEnemyList)
				if (!enemy.EnemyMain.HealthSystem.IsDead()) // Alive
					enemy.Damage(Player.Instance, 1000f);
		}

		private enum Stage
		{
			_0,
			_1,
			_2,
			_3
		}
	}
}
