#region Info
// -----------------------------------------------------------------------
// UI_HotkeyBar.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class UI_HotkeyBar : MonoBehaviour
{

	private Transform abilitySlotTemplate;
	private HotkeyAbilitySystem hotkeyAbilitySystem;

	private void Awake()
	{
		abilitySlotTemplate = transform.Find("abilitySlotTemplate");
		abilitySlotTemplate.gameObject.SetActive(false);
	}

	public void SetHotkeyAbilitySystem(HotkeyAbilitySystem hotkeyAbilitySystem)
	{
		this.hotkeyAbilitySystem = hotkeyAbilitySystem;

		hotkeyAbilitySystem.OnAbilityListChanged
				+= HotkeyAbilitySystem_OnAbilityListChanged;

		UpdateVisual();
	}

	private void HotkeyAbilitySystem_OnAbilityListChanged(
			object sender, EventArgs e)
	{
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		// Clear old objects
		foreach (Transform child in transform)
		{
			if (child == abilitySlotTemplate)
				continue; // Don't destroy Template
			Destroy(child.gameObject);
		}

		var hotkeyAbilityList = hotkeyAbilitySystem.GetHotkeyAbilityList();
		for (var i = 0; i < hotkeyAbilityList.Count; i++)
		{
			var hotkeyAbility = hotkeyAbilityList[i];
			var abilitySlotTransform
					= Instantiate(abilitySlotTemplate, transform);
			abilitySlotTransform.gameObject.SetActive(true);
			var abilitySlotRectTransform
					= abilitySlotTransform.GetComponent<RectTransform>();
			abilitySlotRectTransform.anchoredPosition
					= new Vector2(100f * i, 0f);
			abilitySlotTransform.Find("itemImage").GetComponent<Image>().sprite
					= hotkeyAbility.GetSprite();
			abilitySlotTransform.Find("numberText")
					.GetComponent<TextMeshProUGUI>()
					.SetText((i + 1).ToString());

			abilitySlotTransform.GetComponent<UI_HotkeyBarAbilitySlot>()
					.Setup(hotkeyAbilitySystem, i, hotkeyAbility);
		}

		// Set up extras
		hotkeyAbilityList = hotkeyAbilitySystem.GetExtraHotkeyAbilityList();
		for (var i = 0; i < hotkeyAbilityList.Count; i++)
		{
			var hotkeyAbility = hotkeyAbilityList[i];
			var abilitySlotTransform
					= Instantiate(abilitySlotTemplate, transform);
			abilitySlotTransform.gameObject.SetActive(true);
			var abilitySlotRectTransform
					= abilitySlotTransform.GetComponent<RectTransform>();
			abilitySlotRectTransform.anchoredPosition
					= new Vector2(600f + 100f * i, 0f);
			abilitySlotTransform.Find("itemImage").GetComponent<Image>().sprite
					= hotkeyAbility.GetSprite();
			abilitySlotTransform.Find("numberText")
					.GetComponent<TextMeshProUGUI>()
					.SetText("");

			abilitySlotTransform.GetComponent<UI_HotkeyBarAbilitySlot>()
					.Setup(hotkeyAbilitySystem, -1, hotkeyAbility);
		}
	}
}
