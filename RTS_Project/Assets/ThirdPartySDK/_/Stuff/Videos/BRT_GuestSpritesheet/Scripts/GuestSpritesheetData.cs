#region Info
// -----------------------------------------------------------------------
// GuestSpritesheetData.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class GuestSpritesheetData
{
	public int beardIndex;
	public int bodyIndex;
	public Color bodyPrimaryColor;
	public Color bodySecondaryColor;

	public Color hairColor;

	public int hairIndex;
	public Color skinColor;

	public void Save()
	{
		var jsonString = JsonUtility.ToJson(this);
		PlayerPrefs.SetString("GuestSpritesheetData", jsonString);
		Debug.Log(jsonString);
	}

	public static GuestSpritesheetData Load_Static()
	{
		var jsonString = PlayerPrefs.GetString("GuestSpritesheetData");
		Debug.Log(jsonString);
		return JsonUtility.FromJson<GuestSpritesheetData>(jsonString);
	}

	public static GuestSpritesheetData GenerateRandom()
	{
		Color[] skinColorArray =
		{
			UtilsClass.GetColorFromString("FFE9C6"),
			UtilsClass.GetColorFromString("FFD8A0"),
			UtilsClass.GetColorFromString("D8C19F"),
			UtilsClass.GetColorFromString("D8AC6C"),
			UtilsClass.GetColorFromString("D89774"),
			UtilsClass.GetColorFromString("D1925F"),
			UtilsClass.GetColorFromString("BF8759"),
			UtilsClass.GetColorFromString("86644C"),
			UtilsClass.GetColorFromString("3D2D22")
		};
		var skinColor = skinColorArray[Random.Range(0, skinColorArray.Length)];

		Color[] hairColorArray =
		{
			UtilsClass.GetColorFromString("503D30"),
			UtilsClass.GetColorFromString("D4B60C"),
			UtilsClass.GetColorFromString("5B4636"),
			UtilsClass.GetColorFromString("000000"),
			UtilsClass.GetColorFromString("5B5B5B"),
			UtilsClass.GetColorFromString("BCBCBC"),
			UtilsClass.GetColorFromString("564336")
		};
		var hairColor = hairColorArray[Random.Range(0, hairColorArray.Length)];

		var bodyPrimaryColor = Color.red;
		var bodySecondaryColor = Color.yellow;

		int hairIndex;
		var hasHair = Random.Range(0, 100) < 70;
		if (hasHair)
			hairIndex = Random.Range(0, 4);
		else
			hairIndex = -1;

		int beardIndex;
		var hasBeard = Random.Range(0, 100) < 70;
		if (hasBeard)
			beardIndex = Random.Range(0, 4);
		else
			beardIndex = -1;

		var bodyIndex = Random.Range(0, 4);

		return new GuestSpritesheetData
		{
			bodyIndex = bodyIndex,
			beardIndex = beardIndex,
			hairIndex = hairIndex,

			bodyPrimaryColor = bodyPrimaryColor,
			bodySecondaryColor = bodySecondaryColor,

			skinColor = skinColor,
			hairColor = hairColor
		};
	}
}
