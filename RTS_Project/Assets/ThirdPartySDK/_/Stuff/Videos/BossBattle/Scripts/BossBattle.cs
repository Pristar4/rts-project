#region Info
// -----------------------------------------------------------------------
// BossBattle.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using TopDownShooter;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion
public class BossBattle : MonoBehaviour
{

	public enum Stage
	{
		WaitingToStart,
		Stage_1,
		Stage_2,
		Stage_3
	}

	[SerializeField] private CaptureOnTriggerEnter2D colliderTrigger;
	[SerializeField] private Transform pfHealthPickup;
	[SerializeField] private EnemySpawn pfEnemyShooterSpawn;
	[SerializeField] private EnemySpawn pfEnemyArcherSpawn;
	[SerializeField] private EnemySpawn pfEnemyChargerSpawn;
	[SerializeField] private EnemyTurretLogic enemyTurret;
	[SerializeField] private GameObject shield_1;
	[SerializeField] private GameObject shield_2;

	private List<EnemySpawn> enemySpawnList;
	private List<Vector3> spawnPositionList;
	private Stage stage;

	private void Awake()
	{
		enemySpawnList = new List<EnemySpawn>();
		spawnPositionList = new List<Vector3>();

		foreach (Transform spawnPosition in transform.Find("spawnPositions"))
			spawnPositionList.Add(spawnPosition.position);

		stage = Stage.WaitingToStart;
	}

	private void Start()
	{
		colliderTrigger.OnPlayerTriggerEnter2D
				+= ColliderTrigger_OnPlayerEnterTrigger;

		enemyTurret.GetHealthSystem().OnDamaged += BossBattle_OnDamaged;
		enemyTurret.GetHealthSystem().OnDead += BossBattle_OnDead;

		shield_1.SetActive(false);
		shield_2.SetActive(false);
	}

	public event EventHandler OnBossBattleStarted;
	public event EventHandler OnBossBattleOver;

	private void BossBattle_OnDead(object sender, EventArgs e)
	{
		// Turret dead! Boss battle over!
		Debug.Log("Boss Battle Over!");

		FunctionPeriodic.StopAllFunc("spawnEnemy");
		FunctionPeriodic.StopAllFunc("spawnEnemy_2");
		FunctionPeriodic.StopAllFunc("spawnHealthPickup");

		DestroyAllEnemies();

		OnBossBattleOver?.Invoke(this, EventArgs.Empty);
	}

	private void BossBattle_OnDamaged(object sender, EventArgs e)
	{
		// Turret took damage
		switch (stage)
		{
			case Stage.Stage_1:
				if (enemyTurret.GetHealthSystem().GetHealthNormalized() <=
				    .7f) // Turret under 70% health
					StartNextStage();
				break;
			case Stage.Stage_2:
				if (enemyTurret.GetHealthSystem().GetHealthNormalized() <=
				    .3f) // Turret under 30% health
					StartNextStage();
				break;
		}
	}

	private void
			ColliderTrigger_OnPlayerEnterTrigger(object sender, EventArgs e)
	{
		StartBattle();
		colliderTrigger.OnPlayerTriggerEnter2D
				-= ColliderTrigger_OnPlayerEnterTrigger;
	}

	private void StartBattle()
	{
		Debug.Log("StartBattle");
		StartNextStage();

		OnBossBattleStarted?.Invoke(this, EventArgs.Empty);
	}

	private void StartNextStage()
	{
		switch (stage)
		{
			case Stage.WaitingToStart:
				stage = Stage.Stage_1;

				SpawnEnemy();
				SpawnEnemy();

				FunctionPeriodic.Create(SpawnEnemy, 4f, "spawnEnemy");
				FunctionPeriodic.Create(SpawnHealthPickup, 3f,
						"spawnHealthPickup");
				break;
			case Stage.Stage_1:
				stage = Stage.Stage_2;
				shield_1.SetActive(true);
				SpawnHealthPickup();
				SpawnEnemy();
				SpawnEnemy();
				FunctionTimer.Create(SpawnEnemy, .5f);
				FunctionTimer.Create(SpawnEnemy, 1.5f);
				FunctionTimer.Create(SpawnEnemy, 4.5f);
				FunctionTimer.Create(SpawnEnemy, 6.5f);
				break;
			case Stage.Stage_2:
				stage = Stage.Stage_3;
				shield_2.SetActive(true);
				SpawnHealthPickup();
				SpawnHealthPickup();
				SpawnEnemy();
				SpawnEnemy();
				SpawnEnemy();
				FunctionTimer.Create(SpawnEnemy, .5f);
				FunctionTimer.Create(SpawnEnemy, 1.5f);
				FunctionTimer.Create(SpawnEnemy, 4.5f);
				FunctionTimer.Create(SpawnEnemy, 6.5f);
				FunctionTimer.Create(SpawnEnemy, 7.5f);
				FunctionTimer.Create(SpawnEnemy, 8.5f);
				FunctionPeriodic.Create(SpawnEnemy, 3f, "spawnEnemy_2");
				break;
		}
		Debug.Log("Starting next stage: " + stage);
	}

	private void SpawnEnemy()
	{
		var aliveCount = 0;
		foreach (var enemySpawned in enemySpawnList)
			if (enemySpawned.IsAlive())
			{
				// Enemy alive
				aliveCount++;
				if (aliveCount >= 7) // Don't spawn more enemies
					return;
			}

		var spawnPosition
				= spawnPositionList[Random.Range(0, spawnPositionList.Count)];

		EnemySpawn pfEnemySpawn;
		var rnd = Random.Range(0, 100);

		pfEnemySpawn = pfEnemyArcherSpawn;
		if (rnd < 65) pfEnemySpawn = pfEnemyChargerSpawn;
		if (rnd < 15) pfEnemySpawn = pfEnemyShooterSpawn;

		var enemySpawn
				= Instantiate(pfEnemySpawn, spawnPosition, Quaternion.identity);
		enemySpawn.Spawn();

		enemySpawnList.Add(enemySpawn);
	}

	private void SpawnHealthPickup()
	{
		var spawnPosition
				= spawnPositionList[Random.Range(0, spawnPositionList.Count)];
		var healthPickupTransform
				= Instantiate(pfHealthPickup, spawnPosition,
						Quaternion.identity);
	}

	private void DestroyAllEnemies()
	{
		foreach (var enemySpawn in enemySpawnList)
			if (enemySpawn.IsAlive())
				enemySpawn.KillEnemy();
	}
}
