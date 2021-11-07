#region Info
// -----------------------------------------------------------------------
// Window_CharacterPortrait.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class Window_CharacterPortrait : MonoBehaviour
{

	private static Dictionary<Character, Window_CharacterPortrait>
			windowDictionary;

	private Transform cameraTransform;
	private Character character;
	private Text conText;
	private Text dexText;
	private Transform experienceBar;
	private Transform followTransform;

	private Text levelText;
	private Text strText;
	private Text wisText;

	private void Awake()
	{
		cameraTransform = transform.Find("camera");

		transform.Find("closeBtn").GetComponent<Button_UI>().ClickFunc
				= DestroyWindow;

		levelText = transform.Find("levelText").GetComponent<Text>();
		strText = transform.Find("strText").GetComponent<Text>();
		dexText = transform.Find("dexText").GetComponent<Text>();
		conText = transform.Find("conText").GetComponent<Text>();
		wisText = transform.Find("wisText").GetComponent<Text>();

		experienceBar = transform.Find("experienceBar");
	}

	private void Update()
	{
		cameraTransform.position = new Vector3(followTransform.position.x,
				followTransform.position.y, Camera.main.transform.position.z);
	}

	private void UpdateExperienceBar()
	{
		experienceBar.localScale
				= new Vector3(character.GetExperienceNormalized(), 1, 1);
	}

	private void UpdateStats()
	{
		levelText.text = "Level: " + character.level;
		strText.text = character.STR.ToString();
		dexText.text = character.DEX.ToString();
		conText.text = character.CON.ToString();
		wisText.text = character.WIS.ToString();
	}

	private void Show(Character character)
	{
		this.character = character;
		followTransform = character.transform;

		var renderTexture = new RenderTexture(512, 512, 16);
		transform.Find("camera").GetComponent<Camera>().targetTexture
				= renderTexture;
		transform.Find("rawImage").GetComponent<RawImage>().texture
				= renderTexture;

		transform.Find("nameText").GetComponent<Text>().text = character.name;

		UpdateExperienceBar();
		UpdateStats();

		character.OnExperienceGained += delegate { UpdateExperienceBar(); };
		character.OnLeveledUp += delegate
		{
			UpdateExperienceBar();
			UpdateStats();
		};
	}

	private void DestroyWindow()
	{
		windowDictionary.Remove(character);
		Destroy(gameObject);
	}

	public static void Show_Static(Character character)
	{
		if (windowDictionary == null)
			windowDictionary
					= new Dictionary<Character, Window_CharacterPortrait>();

		if (!windowDictionary.ContainsKey(character))
		{
			var windowCharacterPortraitTransform = Instantiate(
					CharacterPortrait_GameHandler.instance
							.pfWindow_CharacterPortrait);
			windowCharacterPortraitTransform.SetParent(
					CharacterPortrait_GameHandler.instance.canvas.transform,
					false);
			windowCharacterPortraitTransform.GetComponent<RectTransform>()
					.anchoredPosition = new Vector2(Random.Range(-500, 500),
					Random.Range(-200, 200));

			var windowCharacterPortrait = windowCharacterPortraitTransform
					.GetComponent<Window_CharacterPortrait>();
			windowCharacterPortrait.Show(character);

			windowDictionary[character] = windowCharacterPortrait;
		}
	}
}
