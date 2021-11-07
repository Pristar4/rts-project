#region Info
// -----------------------------------------------------------------------
// EnemyTargeting.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

/*
 * Handles finding Enemy Targets
 * */
namespace TopDownShooter
{
	public class EnemyTargeting : MonoBehaviour
	{
		private Enemy.IEnemyTargetable activeEnemyTarget;

		private EnemyMain enemyMain;
		private Func<Enemy.IEnemyTargetable> getEnemyTarget;

		private void Awake()
		{
			enemyMain = GetComponent<EnemyMain>();
		}

		private void Start()
		{
			if (getEnemyTarget == null) SetGetTarget(() => Player.Instance);
		}

		private void Update()
		{
			FindTarget();
		}

		public void SetGetTarget(Func<Enemy.IEnemyTargetable> getEnemyTarget)
		{
			this.getEnemyTarget = getEnemyTarget;
		}

		private void FindTarget()
		{
			var targetRange = enemyMain.EnemyStats.targetRange;
			activeEnemyTarget = null;
			if (getEnemyTarget != null)
				if (Vector3.Distance(getEnemyTarget().GetPosition(),
						    enemyMain.GetPosition()) <
				    targetRange) // Target within range
					activeEnemyTarget = getEnemyTarget();
		}

		public Enemy.IEnemyTargetable GetActiveTarget()
		{
			return activeEnemyTarget;
		}
	}
}
