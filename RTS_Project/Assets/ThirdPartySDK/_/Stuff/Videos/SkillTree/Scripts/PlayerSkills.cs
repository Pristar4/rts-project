#region Info
// -----------------------------------------------------------------------
// PlayerSkills.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
#endregion
public class PlayerSkills
{

	public enum SkillType
	{
		None,
		Earthshatter,
		Whirlwind,
		MoveSpeed_1,
		MoveSpeed_2,
		HealthMax_1,
		HealthMax_2
	}

	private readonly List<SkillType> unlockedSkillTypeList;
	private int skillPoints;

	public PlayerSkills()
	{
		unlockedSkillTypeList = new List<SkillType>();
	}

	public event EventHandler OnSkillPointsChanged;
	public event EventHandler<OnSkillUnlockedEventArgs> OnSkillUnlocked;

	public void AddSkillPoint()
	{
		skillPoints++;
		OnSkillPointsChanged?.Invoke(this, EventArgs.Empty);
	}

	public int GetSkillPoints()
	{
		return skillPoints;
	}

	private void UnlockSkill(SkillType skillType)
	{
		if (!IsSkillUnlocked(skillType))
		{
			unlockedSkillTypeList.Add(skillType);
			OnSkillUnlocked?.Invoke(this,
					new OnSkillUnlockedEventArgs { skillType = skillType });
		}
	}

	public bool IsSkillUnlocked(SkillType skillType)
	{
		return unlockedSkillTypeList.Contains(skillType);
	}

	public bool CanUnlock(SkillType skillType)
	{
		var skillRequirement = GetSkillRequirement(skillType);

		if (skillRequirement != SkillType.None)
		{
			if (IsSkillUnlocked(skillRequirement))
				return true;
			return false;
		}
		return true;
	}

	public SkillType GetSkillRequirement(SkillType skillType)
	{
		switch (skillType)
		{
			case SkillType.HealthMax_2:  return SkillType.HealthMax_1;
			case SkillType.MoveSpeed_2:  return SkillType.MoveSpeed_1;
			case SkillType.Earthshatter: return SkillType.Whirlwind;
		}
		return SkillType.None;
	}

	public bool TryUnlockSkill(SkillType skillType)
	{
		if (CanUnlock(skillType))
		{
			if (skillPoints > 0)
			{
				skillPoints--;
				OnSkillPointsChanged?.Invoke(this, EventArgs.Empty);
				UnlockSkill(skillType);
				return true;
			}
			return false;
		}
		return false;
	}
	public class OnSkillUnlockedEventArgs : EventArgs
	{
		public SkillType skillType;
	}
}
