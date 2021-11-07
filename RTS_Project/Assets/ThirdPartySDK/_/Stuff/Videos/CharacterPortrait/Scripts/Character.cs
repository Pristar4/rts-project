#region Info
// -----------------------------------------------------------------------
// Character.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion
public class Character
{
	public int CON;
	public int DEX;
	public int experience;
	public int experienceMax;

	public int level;
	public string name;

	public int STR;

	public Transform transform;
	public int WIS;

	public Character(Transform transform, string name)
	{
		this.transform = transform;
		this.name = name;
		experience = Random.Range(0, 100);
		experienceMax = 100;

		level = Random.Range(0, 10);

		STR = Random.Range(0, 10);
		DEX = Random.Range(0, 10);
		CON = Random.Range(0, 10);
		WIS = Random.Range(0, 10);

		FunctionPeriodic.Create(AddExperience, .025f);
	}

	public event EventHandler OnLeveledUp;
	public event EventHandler OnExperienceGained;

	private void AddExperience()
	{
		experience++;
		if (experience >= experienceMax)
		{
			experience = 0;
			level++;
			switch (Random.Range(0, 4))
			{
				case 0:
					STR++;
					break;
				case 1:
					DEX++;
					break;
				case 2:
					CON++;
					break;
				case 3:
					WIS++;
					break;
			}
			if (OnLeveledUp != null) OnLeveledUp(this, EventArgs.Empty);
		}
		if (OnExperienceGained != null)
			OnExperienceGained(this, EventArgs.Empty);
	}

	public float GetExperienceNormalized()
	{
		return experience * 1f / experienceMax;
	}
}
