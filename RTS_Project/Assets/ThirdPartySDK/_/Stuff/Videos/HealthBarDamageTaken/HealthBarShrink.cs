#region Info
// -----------------------------------------------------------------------
// HealthBarShrink.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class HealthBarShrink : MonoBehaviour
{

	private const float DAMAGED_HEALTH_SHRINK_TIMER_MAX = .6f;

	private Image barImage;
	private Image damagedBarImage;
	private float damagedHealthShrinkTimer;
	private HealthSystem healthSystem;

	private void Awake()
	{
		barImage = transform.Find("bar").GetComponent<Image>();
		damagedBarImage = transform.Find("damagedBar").GetComponent<Image>();
	}

	private void Start()
	{
		healthSystem = new HealthSystem(100);
		SetHealth(healthSystem.GetHealthNormalized());
		damagedBarImage.fillAmount = barImage.fillAmount;

		healthSystem.OnDamaged += HealthSystem_OnDamaged;
		healthSystem.OnHealed += HealthSystem_OnHealed;

		transform.Find("damageBtn").GetComponent<Button_UI>().ClickFunc
				= () => healthSystem.Damage(10);
		transform.Find("healBtn").GetComponent<Button_UI>().ClickFunc
				= () => healthSystem.Heal(10);
	}

	private void Update()
	{
		damagedHealthShrinkTimer -= Time.deltaTime;
		if (damagedHealthShrinkTimer < 0)
			if (barImage.fillAmount < damagedBarImage.fillAmount)
			{
				var shrinkSpeed = 1f;
				damagedBarImage.fillAmount -= shrinkSpeed * Time.deltaTime;
			}
	}

	private void HealthSystem_OnHealed(object sender, EventArgs e)
	{
		SetHealth(healthSystem.GetHealthNormalized());
		damagedBarImage.fillAmount = barImage.fillAmount;
	}

	private void HealthSystem_OnDamaged(object sender, EventArgs e)
	{
		damagedHealthShrinkTimer = DAMAGED_HEALTH_SHRINK_TIMER_MAX;
		SetHealth(healthSystem.GetHealthNormalized());
	}

	private void SetHealth(float healthNormalized)
	{
		barImage.fillAmount = healthNormalized;
	}
}
