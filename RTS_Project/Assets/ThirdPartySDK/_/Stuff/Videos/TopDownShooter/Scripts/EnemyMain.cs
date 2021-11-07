#region Info
// -----------------------------------------------------------------------
// EnemyMain.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

/*
 * Enemy Class References
 * */
namespace TopDownShooter
{
	public class EnemyMain : MonoBehaviour
	{

		public Enemy Enemy { get; private set; }

		public EnemyPathfindingMovement EnemyPathfindingMovement
		{
			get;
			private set;
		}

		public EnemyTargeting EnemyTargeting { get; private set; }
		public EnemyStats EnemyStats { get; private set; }
		public Rigidbody2D EnemyRigidbody2D { get; private set; }
		public ICharacterAnims CharacterAnims { get; private set; }
		public IAimShootAnims AimShootAnims { get; private set; }

		public HealthSystem HealthSystem { get; private set; }

		private void Awake()
		{
			Enemy = GetComponent<Enemy>();

			EnemyPathfindingMovement = GetComponent<EnemyPathfindingMovement>();
			EnemyTargeting = GetComponent<EnemyTargeting>();
			EnemyStats = GetComponent<EnemyStats>();
			EnemyRigidbody2D = GetComponent<Rigidbody2D>();
			CharacterAnims = GetComponent<ICharacterAnims>();
			AimShootAnims = GetComponent<IAimShootAnims>();

			HealthSystem = new HealthSystem(100);
		}

		public event EventHandler OnDestroySelf;
		public event EventHandler<OnDamagedEventArgs> OnDamaged;

		public Vector3 GetPosition()
		{
			return transform.position;
		}

		public void DestroySelf()
		{
			OnDestroySelf?.Invoke(this, EventArgs.Empty);
		}

		public void Damage(Player attacker, float damageMultiplier)
		{
			OnDamaged?.Invoke(this, new OnDamagedEventArgs
			{
				attacker = attacker,
				damageMultiplier = damageMultiplier
			});
		}
		public class OnDamagedEventArgs
		{
			public Player attacker;
			public float damageMultiplier;
		}
	}

}
