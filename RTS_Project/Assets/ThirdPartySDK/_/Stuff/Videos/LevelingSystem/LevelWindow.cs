#region Info
// -----------------------------------------------------------------------
// LevelWindow.cs
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
public class LevelWindow : MonoBehaviour
{
	private Image experienceBarImage;
	private LevelSystem levelSystem;
	private LevelSystemAnimated levelSystemAnimated;

	private Text levelText;

	private void Awake()
	{
		levelText = transform.Find("levelText").GetComponent<Text>();
		experienceBarImage = transform.Find("experienceBar").Find("bar")
				.GetComponent<Image>();

		transform.Find("experience5Btn").GetComponent<Button_UI>().ClickFunc
				= () => levelSystem.AddExperience(5);
		transform.Find("experience50Btn").GetComponent<Button_UI>().ClickFunc
				= () => levelSystem.AddExperience(50);
		transform.Find("experience500Btn").GetComponent<Button_UI>().ClickFunc
				= () => levelSystem.AddExperience(500);
	}

	private void SetExperienceBarSize(float experienceNormalized)
	{
		experienceBarImage.fillAmount = experienceNormalized;
	}

	private void SetLevelNumber(int levelNumber)
	{
		levelText.text = "LEVEL\n" + (levelNumber + 1);
	}

	public void SetLevelSystem(LevelSystem levelSystem)
	{
		this.levelSystem = levelSystem;
	}

	public void SetLevelSystemAnimated(LevelSystemAnimated levelSystemAnimated)
	{
		// Set the LevelSystemAnimated object
		this.levelSystemAnimated = levelSystemAnimated;

		// Update the starting values
		SetLevelNumber(levelSystemAnimated.GetLevelNumber());
		SetExperienceBarSize(levelSystemAnimated.GetExperienceNormalized());

		// Surbscribe to the changed events
		levelSystemAnimated.OnExperienceChanged
				+= LevelSystemAnimated_OnExperienceChanged;
		levelSystemAnimated.OnLevelChanged
				+= LevelSystemAnimated_OnLevelChanged;
	}

	private void LevelSystemAnimated_OnLevelChanged(object sender, EventArgs e)
	{
		// Level changed, update text
		SetLevelNumber(levelSystemAnimated.GetLevelNumber());
	}

	private void LevelSystemAnimated_OnExperienceChanged(
			object sender, EventArgs e)
	{
		// Experience changed, update bar size
		SetExperienceBarSize(levelSystemAnimated.GetExperienceNormalized());
	}
}
