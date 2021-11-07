#region Info
// -----------------------------------------------------------------------
// ShieldFieldTransformer.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
#endregion

namespace TopDownShooter
{
	public class ShieldFieldTransformer : MonoBehaviour
	{
		private World_Bar healthBar;
		private HealthSystem healthSystem;

		private SpriteRenderer spriteRenderer;

		private void Awake()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			healthSystem = new HealthSystem(40);
			healthBar = new World_Bar(transform, new Vector3(0, 1),
					new Vector3(12, 1.5f), Color.grey, Color.red, 1f, 10000,
					new World_Bar.Outline { color = Color.black, size = .5f });
			healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
			healthSystem.OnDead += HealthSystem_OnDead;
		}

		public event EventHandler OnDestroyed;

		private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
		{
			healthBar.SetSize(healthSystem.GetHealthNormalized());
		}

		private void HealthSystem_OnDead(object sender, EventArgs e)
		{
			//spriteRenderer.sprite = GameAssets.i.s_ShieldTransformerDestroyed;
			//spriteRenderer.material = GameAssets.i.m_SpritesDefault;
			healthBar.Hide();

			OnDestroyed?.Invoke(this, EventArgs.Empty);
		}

		public void Damage(Player attacker, float damageMultiplier)
		{
			if (healthSystem.IsDead()) return; // Already dead
			healthSystem.Damage(Mathf.RoundToInt(10 * damageMultiplier));
		}

		public bool IsAlive()
		{
			return !healthSystem.IsDead();
		}
	}
}
