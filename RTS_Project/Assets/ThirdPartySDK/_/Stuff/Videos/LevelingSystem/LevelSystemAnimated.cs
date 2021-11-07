#region Info
// -----------------------------------------------------------------------
// LevelSystemAnimated.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class LevelSystemAnimated
{
	private readonly float updateTimerMax;
	private int experience;
	private bool isAnimating;

	private int level;

	private LevelSystem levelSystem;
	private float updateTimer;

	public LevelSystemAnimated(LevelSystem levelSystem)
	{
		SetLevelSystem(levelSystem);
		updateTimerMax = .010f;

		FunctionUpdater.Create(() => Update());
	}

	public event EventHandler OnExperienceChanged;
	public event EventHandler OnLevelChanged;

	public void SetLevelSystem(LevelSystem levelSystem)
	{
		this.levelSystem = levelSystem;

		level = levelSystem.GetLevelNumber();
		experience = levelSystem.GetExperience();

		levelSystem.OnExperienceChanged += LevelSystem_OnExperienceChanged;
		levelSystem.OnLevelChanged += LevelSystem_OnLevelChanged;
	}

	private void LevelSystem_OnLevelChanged(object sender, EventArgs e)
	{
		isAnimating = true;
	}

	private void LevelSystem_OnExperienceChanged(object sender, EventArgs e)
	{
		isAnimating = true;
	}

	private void Update()
	{
		if (isAnimating)
		{
			// Check if its time to update
			updateTimer += Time.deltaTime;
			while (updateTimer > updateTimerMax)
			{
				// Time to update
				updateTimer -= updateTimerMax;
				UpdateAddExperience();
			}
		}
	}

	private void UpdateAddExperience()
	{
		if (level < levelSystem.GetLevelNumber())
		{
			// Local level under target level
			AddExperience();
		}
		else
		{
			// Local level equals the target level
			if (experience < levelSystem.GetExperience())
				AddExperience();
			else
				isAnimating = false;
		}
	}

	private void AddExperience()
	{
		experience++;
		if (experience >= levelSystem.GetExperienceToNextLevel(level))
		{
			level++;
			experience = 0;
			if (OnLevelChanged != null) OnLevelChanged(this, EventArgs.Empty);
		}
		if (OnExperienceChanged != null)
			OnExperienceChanged(this, EventArgs.Empty);
	}

	public int GetLevelNumber()
	{
		return level;
	}

	public float GetExperienceNormalized()
	{
		if (levelSystem.IsMaxLevel(level))
			return 1f;
		return (float)experience / levelSystem.GetExperienceToNextLevel(level);
	}
}
