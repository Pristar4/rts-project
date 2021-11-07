#region Info
// -----------------------------------------------------------------------
// PlayerSkillTree.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using TMPro;
using UnityEngine;
#endregion
public class PlayerSkillTree : MonoBehaviour
{

	[SerializeField] private ExperienceBar experienceBar;
	[SerializeField] private TextMeshProUGUI levelText;
	private LevelSystem levelSystem;
	private LevelSystemAnimated levelSystemAnimated;
	private PlayerSkills playerSkills;

	private PlayerSword playerSword;

	private void Awake()
	{
		playerSword = GetComponent<PlayerSword>();
		levelSystem = new LevelSystem();
		levelSystemAnimated = new LevelSystemAnimated(levelSystem);
		playerSkills = new PlayerSkills();
		playerSkills.OnSkillUnlocked += PlayerSkills_OnSkillUnlocked;
	}

	private void Start()
	{
		playerSword.OnEnemyKilled += PlayerSword_OnEnemyKilled;
		levelSystemAnimated.OnExperienceChanged
				+= LevelSystemAnimated_OnExperienceChanged;
		levelSystemAnimated.OnLevelChanged
				+= LevelSystemAnimated_OnLevelChanged;
		levelText.SetText((levelSystemAnimated.GetLevelNumber() + 1)
				.ToString());
	}

	private void PlayerSkills_OnSkillUnlocked(object sender,
	                                          PlayerSkills.
			                                          OnSkillUnlockedEventArgs
			                                          e)
	{
		switch (e.skillType)
		{
			case PlayerSkills.SkillType.MoveSpeed_1:
				SetMovementSpeed(65f);
				break;
			case PlayerSkills.SkillType.MoveSpeed_2:
				SetMovementSpeed(80f);
				break;
			case PlayerSkills.SkillType.HealthMax_1:
				SetHealthAmountMax(12);
				break;
			case PlayerSkills.SkillType.HealthMax_2:
				SetHealthAmountMax(15);
				break;
		}
	}

	public PlayerSkills GetPlayerSkills()
	{
		return playerSkills;
	}

	private void LevelSystemAnimated_OnLevelChanged(object sender, EventArgs e)
	{
		// Level Up
		levelText.SetText((levelSystemAnimated.GetLevelNumber() + 1)
				.ToString());
		//SetHealthAmountMax(8 + levelSystemAnimated.GetLevelNumber());
		playerSkills.AddSkillPoint();
	}

	private void LevelSystemAnimated_OnExperienceChanged(
			object sender, EventArgs e)
	{
		experienceBar.SetSize(levelSystemAnimated.GetExperienceNormalized());
	}

	private void PlayerSword_OnEnemyKilled(object sender, EventArgs e)
	{
		levelSystem.AddExperience(30);
	}

	public bool CanUseEarthshatter()
	{
		return playerSkills.IsSkillUnlocked(PlayerSkills.SkillType
				.Earthshatter);
	}

	public bool CanUseWhirlwind()
	{
		return playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Whirlwind);
	}

	private void SetMovementSpeed(float movementSpeed)
	{
		//playerSword.SetMovementSpeed(movementSpeed);
	}

	private void SetHealthAmountMax(int healthAmountMax)
	{
		//playerSword.SetHealthAmountMax(healthAmountMax);
	}
}
