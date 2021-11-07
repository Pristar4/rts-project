#region Info
// -----------------------------------------------------------------------
// EnemyDropLoot.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

namespace TopDownShooter
{
	public class EnemyDropLoot : MonoBehaviour
	{

		[SerializeField] private Transform lootTransform;
		private EnemyMain enemyMain;

		private void Awake()
		{
			enemyMain = GetComponent<EnemyMain>();
		}

		private void Start()
		{
			enemyMain.OnDestroySelf += EnemyMain_OnDestroySelf;
		}

		private void EnemyMain_OnDestroySelf(object sender, EventArgs e)
		{
			Instantiate(lootTransform, enemyMain.GetPosition(),
					Quaternion.identity);
		}
	}

}
