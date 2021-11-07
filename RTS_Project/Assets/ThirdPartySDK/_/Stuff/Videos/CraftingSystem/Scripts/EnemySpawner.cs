#region Info
// -----------------------------------------------------------------------
// EnemySpawner.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion
public class EnemySpawner : MonoBehaviour
{

	[SerializeField] private Transform pfEnemy;

	private void Start()
	{
		GetComponent<CaptureOnTriggerEnter2D>().OnPlayerTriggerEnter2D
				+= EnemySpawner_OnPlayerTriggerEnter2D;
	}

	private void EnemySpawner_OnPlayerTriggerEnter2D(object sender, EventArgs e)
	{
		FunctionTimer.Create(SpawnEnemy, .1f);
		FunctionTimer.Create(SpawnEnemy, .3f);
		FunctionTimer.Create(SpawnEnemy, .6f);
		FunctionTimer.Create(SpawnEnemy, .8f);
		FunctionTimer.Create(SpawnEnemy, 1.1f);
		FunctionTimer.Create(SpawnEnemy, 1.5f);
	}

	private void SpawnEnemy()
	{
		Instantiate(pfEnemy,
				transform.position +
				UtilsClass.GetRandomDir() * Random.Range(50, 100f),
				Quaternion.identity);
	}
}
