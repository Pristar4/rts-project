#region Info
// -----------------------------------------------------------------------
// UI_SkillTree.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class UI_SkillTree : MonoBehaviour
{

	[SerializeField] private Material skillLockedMaterial;
	[SerializeField] private Material skillUnlockableMaterial;
	[SerializeField] private SkillUnlockPath[] skillUnlockPathArray;
	[SerializeField] private Sprite lineSprite;
	[SerializeField] private Sprite lineGlowSprite;

	private PlayerSkills playerSkills;
	private List<SkillButton> skillButtonList;
	private TextMeshProUGUI skillPointsText;

	private void Awake()
	{
		skillPointsText = transform.Find("skillPointsText")
				.GetComponent<TextMeshProUGUI>();
	}

	public void SetPlayerSkills(PlayerSkills playerSkills)
	{
		this.playerSkills = playerSkills;

		skillButtonList = new List<SkillButton>();
		skillButtonList.Add(new SkillButton(transform.Find("earthshatterBtn"),
				playerSkills, PlayerSkills.SkillType.Earthshatter,
				skillLockedMaterial,
				skillUnlockableMaterial));
		skillButtonList.Add(new SkillButton(transform.Find("whirlwindBtn"),
				playerSkills, PlayerSkills.SkillType.Whirlwind,
				skillLockedMaterial,
				skillUnlockableMaterial));
		skillButtonList.Add(new SkillButton(transform.Find("moveSpeed1Btn"),
				playerSkills, PlayerSkills.SkillType.MoveSpeed_1,
				skillLockedMaterial,
				skillUnlockableMaterial));
		skillButtonList.Add(new SkillButton(transform.Find("moveSpeed2Btn"),
				playerSkills, PlayerSkills.SkillType.MoveSpeed_2,
				skillLockedMaterial,
				skillUnlockableMaterial));
		skillButtonList.Add(new SkillButton(transform.Find("healthMax1Btn"),
				playerSkills, PlayerSkills.SkillType.HealthMax_1,
				skillLockedMaterial,
				skillUnlockableMaterial));
		skillButtonList.Add(new SkillButton(transform.Find("healthMax2Btn"),
				playerSkills, PlayerSkills.SkillType.HealthMax_2,
				skillLockedMaterial,
				skillUnlockableMaterial));

		playerSkills.OnSkillUnlocked += PlayerSkills_OnSkillUnlocked;
		playerSkills.OnSkillPointsChanged += PlayerSkills_OnSkillPointsChanged;

		UpdateVisuals();
		UpdateSkillPoints();
	}

	private void PlayerSkills_OnSkillPointsChanged(object sender, EventArgs e)
	{
		UpdateSkillPoints();
	}

	private void PlayerSkills_OnSkillUnlocked(object sender,
	                                          PlayerSkills.
			                                          OnSkillUnlockedEventArgs
			                                          e)
	{
		UpdateVisuals();
	}

	private void UpdateSkillPoints()
	{
		skillPointsText.SetText(playerSkills.GetSkillPoints().ToString());
	}

	private void UpdateVisuals()
	{
		foreach (var skillButton in skillButtonList) skillButton.UpdateVisual();

		// Darken all links
		foreach (var skillUnlockPath in skillUnlockPathArray)
		foreach (var linkImage in skillUnlockPath.linkImageArray)
		{
			linkImage.color = new Color(.5f, .5f, .5f);
			linkImage.sprite = lineSprite;
		}

		foreach (var skillUnlockPath in skillUnlockPathArray)
			if (playerSkills.IsSkillUnlocked(skillUnlockPath.skillType) ||
			    playerSkills.CanUnlock(skillUnlockPath
					    .skillType)) // Skill unlocked or can be unlocked
				foreach (var linkImage in skillUnlockPath.linkImageArray)
				{
					linkImage.color = Color.white;
					linkImage.sprite = lineGlowSprite;
				}
	}

	/*
	 * Represents a single Skill Button
	 * */
	private class SkillButton
	{
		private readonly Image backgroundImage;
		private readonly Image image;
		private readonly PlayerSkills playerSkills;
		private readonly Material skillLockedMaterial;
		private readonly PlayerSkills.SkillType skillType;
		private readonly Material skillUnlockableMaterial;

		private readonly Transform transform;

		public SkillButton(Transform transform, PlayerSkills playerSkills,
		                   PlayerSkills.SkillType skillType,
		                   Material skillLockedMaterial,
		                   Material skillUnlockableMaterial)
		{
			this.transform = transform;
			this.playerSkills = playerSkills;
			this.skillType = skillType;
			this.skillLockedMaterial = skillLockedMaterial;
			this.skillUnlockableMaterial = skillUnlockableMaterial;

			image = transform.Find("image").GetComponent<Image>();
			backgroundImage
					= transform.Find("background").GetComponent<Image>();

			transform.GetComponent<Button_UI>().ClickFunc = () =>
			{
				if (!playerSkills
						.IsSkillUnlocked(skillType)) // Skill not yet unlocked
					if (!playerSkills.TryUnlockSkill(skillType))
						Tooltip_Warning.ShowTooltip_Static("Cannot unlock " +
								skillType +
								"!");
			};
		}

		public void UpdateVisual()
		{
			if (playerSkills.IsSkillUnlocked(skillType))
			{
				image.material = null;
				backgroundImage.material = null;
			}
			else
			{
				if (playerSkills.CanUnlock(skillType))
				{
					image.material = skillUnlockableMaterial;
					backgroundImage.color
							= UtilsClass.GetColorFromString("4B677D");
					transform.GetComponent<Button_UI>().enabled = true;
				}
				else
				{
					image.material = skillLockedMaterial;
					backgroundImage.color = new Color(.3f, .3f, .3f);
					transform.GetComponent<Button_UI>().enabled = false;
				}
			}
		}
	}


	[Serializable]
	public class SkillUnlockPath
	{
		public PlayerSkills.SkillType skillType;
		public Image[] linkImageArray;
	}
}
