#region Info
// -----------------------------------------------------------------------
// BattleSystem_Done.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
#endregion

/*
 * Handles a Battle, Start, test for End, optional Door
 * */
namespace TopDownShooter
{
	public class BattleSystem_Done : MonoBehaviour
	{

		[SerializeField] private Wave[] waveArray;

		[SerializeField] private CaptureOnTriggerEnter2D startBattleTrigger;
		[SerializeField] private DoorAnims doorAnims;
		private List<EnemySpawn> enemySpawnList;

		private State state;

		private void Awake()
		{
			state = State.WaitingToSpawn;
			enemySpawnList = new List<EnemySpawn>();
		}

		private void Start()
		{
			startBattleTrigger.OnPlayerTriggerEnter2D
					+= StartBattleTrigger_OnPlayerTriggerEnter2D;
		}

		private void Update()
		{
			switch (state)
			{
				case State.Active:
					foreach (var wave in waveArray)
					{
						if (wave.alreadySpawned)
							continue; // Wave already spawned
						wave.time -= Time.deltaTime;
						if (wave.time <= 0f)
						{
							wave.alreadySpawned = true;
							SpawnWave(wave);
						}
					}
					break;
			}
		}

		public event EventHandler OnBattleStarted;
		public event EventHandler OnBattleEnded;

		private void StartBattleTrigger_OnPlayerTriggerEnter2D(
				object sender, EventArgs e)
		{
			StartBattle();
			startBattleTrigger.OnPlayerTriggerEnter2D
					-= StartBattleTrigger_OnPlayerTriggerEnter2D;
		}

		private void SpawnWave(Wave wave)
		{
			var waveSpawnEnemyList = new List<EnemySpawn>();
			if (wave.enemySpawnContainer != null)
				foreach (Transform transform in wave.enemySpawnContainer)
				{
					var enemySpawn = transform.GetComponent<EnemySpawn>();
					if (enemySpawn != null) waveSpawnEnemyList.Add(enemySpawn);
				}

			if (wave.enemySpawnArray != null)
				waveSpawnEnemyList.AddRange(wave.enemySpawnArray);

			foreach (var enemySpawn in waveSpawnEnemyList)
			{
				enemySpawn.Spawn();
				enemySpawn.OnDead += EnemySpawn_OnDead;
				enemySpawnList.Add(enemySpawn);
			}
		}

		private void StartBattle()
		{
			state = State.Active;

			if (doorAnims != null)
			{
				doorAnims.SetColor(DoorAnims.ColorName.Red);
				doorAnims.CloseDoor();
			}

			OnBattleStarted?.Invoke(this, EventArgs.Empty);
		}

		private void EndBattle()
		{
			if (doorAnims != null)
			{
				doorAnims.SetColor(DoorAnims.ColorName.Green);
				FunctionTimer.Create(doorAnims.OpenDoor, 1.5f);
			}

			OnBattleEnded?.Invoke(this, EventArgs.Empty);
		}

		private void EnemySpawn_OnDead(object sender, EventArgs e)
		{
			TestBattleOver();
		}

		private void TestBattleOver()
		{
			foreach (var enemySpawn in enemySpawnList)
				if (enemySpawn.IsAlive()) // Still alive
					return;

			// All dead!
			EndBattle();
		}

		private enum State
		{
			WaitingToSpawn,
			Active,
			BattleOver
		}

		[Serializable]
		public class Wave
		{
			public Transform enemySpawnContainer;
			public EnemySpawn[] enemySpawnArray;
			public float time;
			public bool alreadySpawned;
		}
	}
}
