#region Info
// -----------------------------------------------------------------------
// TextWriter.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class TextWriter : MonoBehaviour
{

	private static TextWriter instance;

	private List<TextWriterSingle> textWriterSingleList;

	private void Awake()
	{
		instance = this;
		textWriterSingleList = new List<TextWriterSingle>();
	}

	private void Update()
	{
		for (var i = 0; i < textWriterSingleList.Count; i++)
		{
			var destroyInstance = textWriterSingleList[i].Update();
			if (destroyInstance)
			{
				textWriterSingleList.RemoveAt(i);
				i--;
			}
		}
	}

	public static TextWriterSingle AddWriter_Static(
			Text uiText, string textToWrite, float timePerCharacter,
			bool invisibleCharacters, bool removeWriterBeforeAdd,
			Action onComplete)
	{
		if (removeWriterBeforeAdd) instance.RemoveWriter(uiText);
		return instance.AddWriter(uiText, textToWrite, timePerCharacter,
				invisibleCharacters, onComplete);
	}

	private TextWriterSingle AddWriter(Text uiText, string textToWrite,
	                                   float timePerCharacter,
	                                   bool invisibleCharacters,
	                                   Action onComplete)
	{
		var textWriterSingle = new TextWriterSingle(uiText, textToWrite,
				timePerCharacter, invisibleCharacters, onComplete);
		textWriterSingleList.Add(textWriterSingle);
		return textWriterSingle;
	}

	public static void RemoveWriter_Static(Text uiText)
	{
		instance.RemoveWriter(uiText);
	}

	private void RemoveWriter(Text uiText)
	{
		for (var i = 0; i < textWriterSingleList.Count; i++)
			if (textWriterSingleList[i].GetUIText() == uiText)
			{
				textWriterSingleList.RemoveAt(i);
				i--;
			}
	}

	/*
	 * Represents a single TextWriter instance
	 * */
	public class TextWriterSingle
	{
		private readonly bool invisibleCharacters;
		private readonly Action onComplete;
		private readonly string textToWrite;
		private readonly float timePerCharacter;

		private readonly Text uiText;
		private int characterIndex;
		private float timer;

		public TextWriterSingle(Text uiText, string textToWrite,
		                        float timePerCharacter,
		                        bool invisibleCharacters,
		                        Action onComplete)
		{
			this.uiText = uiText;
			this.textToWrite = textToWrite;
			this.timePerCharacter = timePerCharacter;
			this.invisibleCharacters = invisibleCharacters;
			this.onComplete = onComplete;
			characterIndex = 0;
		}

		// Returns true on complete
		public bool Update()
		{
			timer -= Time.deltaTime;
			while (timer <= 0f)
			{
				// Display next character
				timer += timePerCharacter;
				characterIndex++;
				var text = textToWrite.Substring(0, characterIndex);
				if (invisibleCharacters)
					text += "<color=#00000000>" +
					        textToWrite.Substring(characterIndex) +
					        "</color>";
				uiText.text = text;

				if (characterIndex >= textToWrite.Length)
				{
					// Entire string displayed
					if (onComplete != null) onComplete();
					return true;
				}
			}

			return false;
		}

		public Text GetUIText()
		{
			return uiText;
		}

		public bool IsActive()
		{
			return characterIndex < textToWrite.Length;
		}

		public void WriteAllAndDestroy()
		{
			uiText.text = textToWrite;
			characterIndex = textToWrite.Length;
			if (onComplete != null) onComplete();
			RemoveWriter_Static(uiText);
		}
	}
}
