#region Info
// -----------------------------------------------------------------------
// HealthSystem.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
#endregion
public class HealthSystem
{
	private int health;

	private int healthMax;

	public HealthSystem(int healthMax)
	{
		this.healthMax = healthMax;
		health = healthMax;
	}

	public event EventHandler OnHealthChanged;
	public event EventHandler OnHealthMaxChanged;
	public event EventHandler OnDamaged;
	public event EventHandler OnHealed;
	public event EventHandler OnDead;

	public int GetHealth()
	{
		return health;
	}

	public int GetHealthMax()
	{
		return healthMax;
	}

	public float GetHealthNormalized()
	{
		return (float)health / healthMax;
	}

	public void Damage(int amount)
	{
		health -= amount;
		if (health < 0) health = 0;
		OnHealthChanged?.Invoke(this, EventArgs.Empty);
		OnDamaged?.Invoke(this, EventArgs.Empty);

		if (health <= 0) Die();
	}

	public void Die()
	{
		OnDead?.Invoke(this, EventArgs.Empty);
	}

	public bool IsDead()
	{
		return health <= 0;
	}

	public void Heal(int amount)
	{
		health += amount;
		if (health > healthMax) health = healthMax;
		OnHealthChanged?.Invoke(this, EventArgs.Empty);
		OnHealed?.Invoke(this, EventArgs.Empty);
	}

	public void HealComplete()
	{
		health = healthMax;
		OnHealthChanged?.Invoke(this, EventArgs.Empty);
		OnHealed?.Invoke(this, EventArgs.Empty);
	}

	public void SetHealthMax(int healthMax, bool fullHealth)
	{
		this.healthMax = healthMax;
		if (fullHealth) health = healthMax;
		OnHealthMaxChanged?.Invoke(this, EventArgs.Empty);
		OnHealthChanged?.Invoke(this, EventArgs.Empty);
	}
}
