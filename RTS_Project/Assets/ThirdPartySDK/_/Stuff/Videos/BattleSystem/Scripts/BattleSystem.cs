#region Info
// -----------------------------------------------------------------------
// BattleSystem.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using TopDownShooter;
using UnityEngine;
#endregion
public class BattleSystem : MonoBehaviour
{

	[SerializeField] private CaptureOnTriggerEnter2D colliderTrigger;
	[SerializeField] private Wave[] waveArray;

	private State state;

	private void Awake()
	{
		state = State.Idle;
	}

	private void Start()
	{
		colliderTrigger.OnPlayerTriggerEnter2D
				+= ColliderTrigger_OnPlayerEnterTrigger;
	}

	private void Update()
	{
		switch (state)
		{
			case State.Active:
				foreach (var wave in waveArray) wave.Update();

				TestBattleOver();
				break;
		}
	}

	public event EventHandler OnBattleStarted;
	public event EventHandler OnBattleOver;

	private void
			ColliderTrigger_OnPlayerEnterTrigger(object sender, EventArgs e)
	{
		if (state == State.Idle)
		{
			StartBattle();
			colliderTrigger.OnPlayerTriggerEnter2D
					-= ColliderTrigger_OnPlayerEnterTrigger;
		}
	}

	private void StartBattle()
	{
		Debug.Log("StartBattle");
		state = State.Active;
		OnBattleStarted?.Invoke(this, EventArgs.Empty);
	}

	private void TestBattleOver()
	{
		if (state == State.Active)
			if (AreWavesOver())
			{
				// Battle is over!
				state = State.BattleOver;
				Debug.Log("Battle Over!");
				OnBattleOver?.Invoke(this, EventArgs.Empty);
			}
	}

	private bool AreWavesOver()
	{
		foreach (var wave in waveArray)
			if (wave.IsWaveOver())
			{
				// Wave is over
			}
			else
			{
				// Wave not over
				return false;
			}

		return true;
	}

	private enum State
	{
		Idle,
		Active,
		BattleOver
	}


	/*
	 * Represents a single Enemy Spawn Wave
	 * */
	[Serializable]
	private class Wave
	{

		[SerializeField] private EnemySpawn[] enemySpawnArray;
		[SerializeField] private float timer;

		public void Update()
		{
			if (timer >= 0)
			{
				timer -= Time.deltaTime;
				if (timer < 0) SpawnEnemies();
			}
		}

		private void SpawnEnemies()
		{
			foreach (var enemySpawn in enemySpawnArray) enemySpawn.Spawn();
		}

		public bool IsWaveOver()
		{
			if (timer < 0)
			{
				// Wave spawned
				foreach (var enemySpawn in enemySpawnArray)
					if (enemySpawn.IsAlive())
						return false;
				return true;
			}
			// Enemies haven't spawned yet
			return false;
		}
	}
}
