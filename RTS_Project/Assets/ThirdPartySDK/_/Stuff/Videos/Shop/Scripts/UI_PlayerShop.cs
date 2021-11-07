#region Info
// -----------------------------------------------------------------------
// UI_PlayerShop.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using TMPro;
using UnityEngine;
#endregion
public class UI_PlayerShop : MonoBehaviour
{

	private TextMeshProUGUI goldText;
	private TextMeshProUGUI healthPotionText;

	private void Awake()
	{
		goldText = transform.Find("goldText").GetComponent<TextMeshProUGUI>();
		healthPotionText = transform.Find("healthPotionText")
				.GetComponent<TextMeshProUGUI>();
	}

	private void Start()
	{
		UpdateText();

		Player.Instance.OnGoldAmountChanged += Instance_OnGoldAmountChanged;
		Player.Instance.OnHealthPotionAmountChanged
				+= Instance_OnHealthPotionAmountChanged;
	}

	private void
			Instance_OnHealthPotionAmountChanged(object sender, EventArgs e)
	{
		UpdateText();
	}

	private void Instance_OnGoldAmountChanged(object sender, EventArgs e)
	{
		UpdateText();
	}

	private void UpdateText()
	{
		goldText.text = Player.Instance.GetGoldAmount().ToString();
		healthPotionText.text
				= Player.Instance.GetHealthPotionAmount().ToString();
	}
}
