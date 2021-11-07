#region Info
// -----------------------------------------------------------------------
// EnemyShuriken.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion

namespace TopDownShooter
{
	public class EnemyShuriken : MonoBehaviour
	{

		private const float MOVE_SPEED = 90f;

		private Enemy enemySpawner;
		private Vector3 moveDir;
		private Transform particleTransform;
		private Rigidbody2D shurikenRigidbody2D;

		private void Awake()
		{
			shurikenRigidbody2D = GetComponent<Rigidbody2D>();
			shurikenRigidbody2D.AddTorque(30f, ForceMode2D.Impulse);
			particleTransform = transform.Find("ParticleSystem");
		}

		private void FixedUpdate()
		{
			shurikenRigidbody2D.velocity = moveDir * MOVE_SPEED;
		}

		private void OnTriggerEnter2D(Collider2D collider)
		{
			//int hitLayerMask = ~(1 << GameAssets.i.enemyLayer | 1 << GameAssets.i.ignoreRaycastLayer);
			var hitLayerMask = ~0;
			var colliderInLayerMask
					= ((1 << collider.gameObject.layer) & hitLayerMask) != 0;
			if (colliderInLayerMask)
			{
				// Touching a target layer
				var player = collider.GetComponent<Player>();
				if (player != null) player.Damage(enemySpawner, 1f);
				particleTransform.SetParent(null);
				particleTransform.GetComponent<ParticleSystem>().Stop();
				Destroy(particleTransform.gameObject, 1f);
				Destroy(gameObject);
				/*
				GetComponent<Collider2D>().enabled = false;
				shurikenRigidbody2D.velocity = Vector2.zero;
				this.enabled = false;
				transform.Find("Sprite").gameObject.SetActive(false);
				*/
			}
		}

		public static EnemyShuriken Create(Enemy enemySpawner,
		                                   Vector3 spawnPosition,
		                                   Vector3 moveDir)
		{
			Transform
					enemyShurikenTransform
							= null; // Instantiate(GameAssets.i.pfEnemyShuriken, spawnPosition, Quaternion.identity);

			var enemyShuriken
					= enemyShurikenTransform.GetComponent<EnemyShuriken>();
			enemyShuriken.Setup(enemySpawner, moveDir);

			return enemyShuriken;
		}

		private void Setup(Enemy enemySpawner, Vector3 moveDir)
		{
			this.enemySpawner = enemySpawner;
			this.moveDir = moveDir;
		}
	}

}
