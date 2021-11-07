#region Info
// -----------------------------------------------------------------------
// LevelSystem.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion
public class LevelSystem
{

	private static readonly int[] experiencePerLevel =
			{ 100, 120, 140, 160, 180, 200, 220, 250, 300, 400 };
	private int experience;

	private int level;

	public LevelSystem()
	{
		level = 0;
		experience = 0;
	}

	public event EventHandler OnExperienceChanged;
	public event EventHandler OnLevelChanged;

	public void AddExperience(int amount)
	{
		if (!IsMaxLevel())
		{
			experience += amount;
			while (!IsMaxLevel() &&
			       experience >= GetExperienceToNextLevel(level))
			{
				// Enough experience to level up
				experience -= GetExperienceToNextLevel(level);
				level++;
				if (OnLevelChanged != null)
					OnLevelChanged(this, EventArgs.Empty);
			}
			if (OnExperienceChanged != null)
				OnExperienceChanged(this, EventArgs.Empty);
		}
	}

	public int GetLevelNumber()
	{
		return level;
	}

	public float GetExperienceNormalized()
	{
		if (IsMaxLevel())
			return 1f;
		return (float)experience / GetExperienceToNextLevel(level);
	}

	public int GetExperience()
	{
		return experience;
	}

	public int GetExperienceToNextLevel(int level)
	{
		if (level < experiencePerLevel.Length) return experiencePerLevel[level];
		// Level Invalid
		Debug.LogError("Level invalid: " + level);
		return 100;
	}

	public bool IsMaxLevel()
	{
		return IsMaxLevel(level);
	}

	public bool IsMaxLevel(int level)
	{
		return level == experiencePerLevel.Length - 1;
	}
}
