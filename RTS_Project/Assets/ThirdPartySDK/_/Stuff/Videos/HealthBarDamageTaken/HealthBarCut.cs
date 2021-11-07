#region Info
// -----------------------------------------------------------------------
// HealthBarCut.cs
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
public class HealthBarCut : MonoBehaviour
{

	private const float BAR_WIDTH = 500f;

	private Image barImage;
	private Transform damagedBarTemplate;
	private HealthSystem healthSystem;

	private void Awake()
	{
		barImage = transform.Find("bar").GetComponent<Image>();
		damagedBarTemplate = transform.Find("damagedBarTemplate");
	}

	private void Start()
	{
		healthSystem = new HealthSystem(100);
		SetHealth(healthSystem.GetHealthNormalized());

		healthSystem.OnDamaged += HealthSystem_OnDamaged;
		healthSystem.OnHealed += HealthSystem_OnHealed;

		transform.Find("damageBtn").GetComponent<Button_UI>().ClickFunc
				= () => healthSystem.Damage(10);
		transform.Find("healBtn").GetComponent<Button_UI>().ClickFunc
				= () => healthSystem.Heal(10);
	}

	private void Update()
	{
	}

	private void HealthSystem_OnHealed(object sender, EventArgs e)
	{
		SetHealth(healthSystem.GetHealthNormalized());
	}

	private void HealthSystem_OnDamaged(object sender, EventArgs e)
	{
		var beforeDamagedBarFillAmount = barImage.fillAmount;
		SetHealth(healthSystem.GetHealthNormalized());
		var damagedBar = Instantiate(damagedBarTemplate, transform);
		damagedBar.gameObject.SetActive(true);
		damagedBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(
				barImage.fillAmount * BAR_WIDTH,
				damagedBar.GetComponent<RectTransform>().anchoredPosition.y);
		damagedBar.GetComponent<Image>().fillAmount
				= beforeDamagedBarFillAmount - barImage.fillAmount;
		damagedBar.gameObject.AddComponent<HealthBarCutFallDown>();
	}

	private void SetHealth(float healthNormalized)
	{
		barImage.fillAmount = healthNormalized;
	}
}
