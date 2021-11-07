#region Info
// -----------------------------------------------------------------------
// SaveSystem.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.IO;
using UnityEngine;
#endregion
public static class SaveSystem
{

	private const string SAVE_EXTENSION = "txt";

	private static readonly string SAVE_FOLDER
			= Application.dataPath + "/Saves/";
	private static bool isInit;

	public static void Init()
	{
		if (!isInit)
		{
			isInit = true;
			// Test if Save Folder exists
			if (!Directory.Exists(SAVE_FOLDER)) // Create Save Folder
				Directory.CreateDirectory(SAVE_FOLDER);
		}
	}

	public static void Save(string fileName, string saveString, bool overwrite)
	{
		Init();
		var saveFileName = fileName;
		if (!overwrite)
		{
			// Make sure the Save Number is unique so it doesnt overwrite a previous save file
			var saveNumber = 1;
			while (File.Exists(
					SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION))
			{
				saveNumber++;
				saveFileName = fileName + "_" + saveNumber;
			}
			// saveFileName is unique
		}
		File.WriteAllText(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION,
				saveString);
	}

	public static string Load(string fileName)
	{
		Init();
		if (File.Exists(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION))
		{
			var saveString
					= File.ReadAllText(SAVE_FOLDER + fileName + "." +
					                   SAVE_EXTENSION);
			return saveString;
		}
		return null;
	}

	public static string LoadMostRecentFile()
	{
		Init();
		var directoryInfo = new DirectoryInfo(SAVE_FOLDER);
		// Get all save files
		var saveFiles = directoryInfo.GetFiles("*." + SAVE_EXTENSION);
		// Cycle through all save files and identify the most recent one
		FileInfo mostRecentFile = null;
		foreach (var fileInfo in saveFiles)
			if (mostRecentFile == null) { mostRecentFile = fileInfo; }
			else
			{
				if (fileInfo.LastWriteTime > mostRecentFile.LastWriteTime)
					mostRecentFile = fileInfo;
			}

		// If theres a save file, load it, if not return null
		if (mostRecentFile != null)
		{
			var saveString = File.ReadAllText(mostRecentFile.FullName);
			return saveString;
		}
		return null;
	}

	public static void SaveObject(object saveObject)
	{
		SaveObject("save", saveObject, false);
	}

	public static void SaveObject(string fileName, object saveObject,
	                              bool overwrite)
	{
		Init();
		var json = JsonUtility.ToJson(saveObject);
		Save(fileName, json, overwrite);
	}

	public static TSaveObject LoadMostRecentObject<TSaveObject>()
	{
		Init();
		var saveString = LoadMostRecentFile();
		if (saveString != null)
		{
			var saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
			return saveObject;
		}
		return default;
	}

	public static TSaveObject LoadObject<TSaveObject>(string fileName)
	{
		Init();
		var saveString = Load(fileName);
		if (saveString != null)
		{
			var saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
			return saveObject;
		}
		return default;
	}
}
