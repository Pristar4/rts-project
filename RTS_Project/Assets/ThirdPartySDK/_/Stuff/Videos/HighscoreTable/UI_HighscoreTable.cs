#region Info
// -----------------------------------------------------------------------
// UI_HighscoreTable.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class UI_HighscoreTable : MonoBehaviour
{

	private Transform entryContainer;
	private Transform entryTemplate;
	private List<Transform> highscoreEntryTransformList;

	public static UI_HighscoreTable Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		GetComponent<RectTransform>().anchoredPosition
				= Vector2.zero; // Reset Position
		GetComponent<RectTransform>().sizeDelta = Vector2.zero; // Reset Size

		entryContainer = transform.Find("highscoreEntryContainer");
		entryTemplate = entryContainer.Find("highscoreEntryTemplate");

		entryTemplate.gameObject.SetActive(false);

		//PlayerPrefs.DeleteKey("highscoreTable");
		var jsonString = PlayerPrefs.GetString("highscoreTable");
		var highscores = JsonUtility.FromJson<Highscores>(jsonString);

		//highscores = null; PlayerPrefs.SetString("highscoreTable", ""); PlayerPrefs.Save(); // Reset Scores

		if (highscores == null)
		{
			// There's no stored table, initialize
			Debug.Log("Initializing table with default values...");
			AddHighscoreEntry(1000000, "CMK");
			AddHighscoreEntry(897621, "JOE");
			AddHighscoreEntry(872931, "DAV");
			AddHighscoreEntry(785123, "CAT");
			AddHighscoreEntry(542024, "MAX");
			AddHighscoreEntry(68245, "AAA");
			// Reload
			jsonString = PlayerPrefs.GetString("highscoreTable");
			highscores = JsonUtility.FromJson<Highscores>(jsonString);
		}

		RefreshHighscoreTable();

		Hide();
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Show()
	{
		gameObject.SetActive(true);
		transform.SetAsLastSibling();
	}

	private void RefreshHighscoreTable()
	{
		var jsonString = PlayerPrefs.GetString("highscoreTable");
		var highscores = JsonUtility.FromJson<Highscores>(jsonString);

		// Sort entry list by Score
		for (var i = 0; i < highscores.highscoreEntryList.Count; i++)
		for (var j = i + 1; j < highscores.highscoreEntryList.Count; j++)
			if (highscores.highscoreEntryList[j].score >
			    highscores.highscoreEntryList[i].score)
			{
				// Swap
				var tmp = highscores.highscoreEntryList[i];
				highscores.highscoreEntryList[i]
						= highscores.highscoreEntryList[j];
				highscores.highscoreEntryList[j] = tmp;
			}

		if (highscoreEntryTransformList != null)
			foreach (var highscoreEntryTransform in highscoreEntryTransformList)
				Destroy(highscoreEntryTransform.gameObject);

		highscoreEntryTransformList = new List<Transform>();
		foreach (var highscoreEntry in highscores.highscoreEntryList)
			CreateHighscoreEntryTransform(highscoreEntry, entryContainer,
					highscoreEntryTransformList);
	}

	private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry,
	                                           Transform container,
	                                           List<Transform> transformList)
	{
		var templateHeight = 31f;
		var entryTransform = Instantiate(entryTemplate, container);
		var entryRectTransform = entryTransform.GetComponent<RectTransform>();
		entryRectTransform.anchoredPosition
				= new Vector2(0, -templateHeight * transformList.Count);
		entryTransform.gameObject.SetActive(true);

		var rank = transformList.Count + 1;
		string rankString;
		switch (rank)
		{
			default:
				rankString = rank + "TH";
				break;

			case 1:
				rankString = "1ST";
				break;
			case 2:
				rankString = "2ND";
				break;
			case 3:
				rankString = "3RD";
				break;
		}

		entryTransform.Find("posText").GetComponent<Text>().text = rankString;

		var score = highscoreEntry.score;

		entryTransform.Find("scoreText").GetComponent<Text>().text
				= score.ToString();

		var name = highscoreEntry.name;
		entryTransform.Find("nameText").GetComponent<Text>().text = name;

		// Set background visible odds and evens, easier to read
		entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

		// Highlight First
		if (rank == 1)
		{
			entryTransform.Find("posText").GetComponent<Text>().color
					= Color.green;
			entryTransform.Find("scoreText").GetComponent<Text>().color
					= Color.green;
			entryTransform.Find("nameText").GetComponent<Text>().color
					= Color.green;
		}

		// Set tropy
		switch (rank)
		{
			default:
				entryTransform.Find("trophy").gameObject.SetActive(false);
				break;
			case 1:
				entryTransform.Find("trophy").GetComponent<Image>().color
						= UtilsClass.GetColorFromString("FFD200");
				break;
			case 2:
				entryTransform.Find("trophy").GetComponent<Image>().color
						= UtilsClass.GetColorFromString("C6C6C6");
				break;
			case 3:
				entryTransform.Find("trophy").GetComponent<Image>().color
						= UtilsClass.GetColorFromString("B76F56");
				break;

		}

		transformList.Add(entryTransform);
	}

	public void AddHighscoreEntry(int score, string name)
	{
		// Create HighscoreEntry
		var highscoreEntry = new HighscoreEntry { score = score, name = name };

		// Load saved Highscores
		var jsonString = PlayerPrefs.GetString("highscoreTable");
		var highscores = JsonUtility.FromJson<Highscores>(jsonString);

		if (highscores == null) // There's no stored table, initialize
			highscores = new Highscores
			{
				highscoreEntryList = new List<HighscoreEntry>()
			};

		// Add new entry to Highscores
		highscores.highscoreEntryList.Add(highscoreEntry);

		// Save updated Highscores
		var json = JsonUtility.ToJson(highscores);
		PlayerPrefs.SetString("highscoreTable", json);
		PlayerPrefs.Save();

		RefreshHighscoreTable();
	}

	private class Highscores
	{
		public List<HighscoreEntry> highscoreEntryList;
	}

	/*
	 * Represents a single High score entry
	 * */
	[Serializable]
	private class HighscoreEntry
	{
		public int score;
		public string name;
	}
}
