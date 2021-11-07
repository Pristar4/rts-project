#region Info
// -----------------------------------------------------------------------
// SaveSystem.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
#endregion

namespace V_AnimationSystem
{

	/*
	 * Animation Save System
	 * */
	public static class SaveSystem
	{









		public static string GetCompressedString(string save)
		{
			return Convert.ToBase64String(
					CLZF2.Compress(Encoding.ASCII.GetBytes(save)));
		}
		public static string GetUnCompressedString(string save)
		{
			return Encoding.ASCII.GetString(
					CLZF2.Decompress(Convert.FromBase64String(save)));
		}





		public static void Save(string folderPath, string saveName,
		                        FileData.FileType fileType, string saveString)
		{
			byte[] fileSaveBytes;
			Save(folderPath, saveName, fileType, saveString, out fileSaveBytes);
		}
		public static void Save(string folderPath, string saveName,
		                        FileData.FileType fileType, string saveString,
		                        out byte[] fileSaveBytes)
		{
			// Save name containing extension
			var saveFile = folderPath + saveName;
			Save(saveFile, fileType, saveString, out fileSaveBytes);
		}

		public static void Save(string fullSavePath, FileData.FileType fileType,
		                        string saveString, out byte[] fileSaveBytes)
		{
			// Save name containing extension
			var saveFile = fullSavePath;
			//Compress Save
			var saveStringCompressed = GetCompressedString(saveString);
			var header = GetCompressedString(Save_Header(saveStringCompressed));

			fileSaveBytes
					= FileData.Save(fileType, header, saveStringCompressed);

			File.WriteAllBytes(saveFile, fileSaveBytes);
		}

		public static bool Load(string folderPath, string file,
		                        out FileData fileData)
		{
			// Byte save
			return Load(folderPath + file, out fileData);
		}
		public static bool Load(string fullFilePath, out FileData fileData)
		{
			// File name containing extension
			// Assumes extension is 3 characters long
			var readAllBytes = File.ReadAllBytes(fullFilePath);

			return Load(readAllBytes, out fileData);
		}
		public static bool Load(byte[] byteArr, out FileData fileData)
		{
			try
			{
				fileData = FileData.Load(byteArr);
				return true;
			}
			catch (Exception e)
			{
				Debug.Log("Load Failed: " + e);
				fileData = default;
				return false;
			}
		}



		private static string Save_Header(string saveContent)
		{
			//Returns a string to be used in savefiles
			return HeaderData.Save(saveContent);
		}
		private static HeaderData Load_Header(string header)
		{
			var headerData = HeaderData.Load(header);
			return headerData;
		}


		public struct HeaderData
		{

			private static readonly string[] prevVersions = { "0.01" };
			private const string version = "1.00";
			private const string versionDate = "12-05-2018";
			private static byte versionByte = (byte)prevVersions.Length;


			public int saveByteLength;
			public string animationVersion;
			public string headerDataVersion;
			public string date;


			public static HeaderData Generate(string saveContent)
			{
				HeaderData headerData;
				headerData.saveByteLength
						= Encoding.UTF8.GetBytes(saveContent).Length;
				headerData.animationVersion = V_Animation.version;
				headerData.headerDataVersion = version;
				headerData.date = DateTime.Now.ToString("dd-MM-yy_HH-mm-ss");
				return headerData;
			}
			public static string Save(string saveContent)
			{
				return Save(Generate(saveContent));
			}
			public static string Save(HeaderData headerData)
			{
				string[] content =
				{
					headerData.saveByteLength + "",
					headerData.animationVersion,
					headerData.date,
					headerData.headerDataVersion
				};
				return string.Join("#HEADER#", content);
			}
			public static HeaderData Load(string header)
			{
				var content = V_Animation.SplitString(header, "#HEADER#");
				HeaderData headerData;
				headerData.saveByteLength = int.Parse(content[0]);
				headerData.animationVersion = content[1];
				headerData.date = content[2];
				headerData.headerDataVersion = content[3];
				return headerData;
			}

			public override string ToString()
			{
				return
						"FILE HEADER" + "\n" +
						"saveByteLength: " + saveByteLength + "\n" +
						"gameVersion: " + animationVersion + "\n" +
						"headerDataVersion: " + headerDataVersion + "\n" +
						"date: " + date + "\n" +
						"";
			}
		}


		public struct FileData
		{

			public enum FileType
			{
				Animation,
				AnimationType,
				UnitInfo,
				MapSpawns,
				GameState_Stats,
				NinjaTycoonSave
			}

			public FileType fileType;
			public HeaderData headerData;
			public string save;

			public static FileData Load(byte[] byteArr)
			{
				var fileType = (FileType)byteArr[0];
				var fileTypeByteAmount = 1;
				byte[] headerSizeBytes = { byteArr[1], byteArr[2] };
				var headerSizeByteAmount = headerSizeBytes.Length;
				var headerSize = BitConverter.ToInt16(headerSizeBytes, 0);

				var headerBytes = new byte[headerSize];
				Array.Copy(byteArr, fileTypeByteAmount + headerSizeByteAmount,
						headerBytes, 0, headerSize);
				var headerCompressed = Encoding.UTF8.GetString(headerBytes);
				var header = GetUnCompressedString(headerCompressed);

				var headerData = HeaderData.Load(header);

				var saveBytes = new byte[headerData.saveByteLength];
				Array.Copy(byteArr,
						fileTypeByteAmount + headerSizeByteAmount +
						headerBytes.Length, saveBytes, 0,
						headerData.saveByteLength);
				var saveCompressed = Encoding.UTF8.GetString(saveBytes);

				var save = GetUnCompressedString(saveCompressed);

				FileData fileData;
				fileData.fileType = fileType;
				fileData.headerData = headerData;
				fileData.save = save;

				return fileData;
			}

			public static byte[] Save(FileType fileType, string header,
			                          string save)
			{
				// Convert header into bytes
				var fileTypeByte = (byte)fileType;
				var headerBytes = Encoding.UTF8.GetBytes(header);

				// Save 2 bytes for header length
				var headerSize = (short)headerBytes.Length;
				var headerSizeBytes = BitConverter.GetBytes(headerSize);

				// Get save bytes
				var saveBytes = Encoding.UTF8.GetBytes(save);

				// Merge all together
				var totalBytes = new List<byte>();
				totalBytes.Add(fileTypeByte);
				totalBytes.AddRange(headerSizeBytes);
				totalBytes.AddRange(headerBytes);
				totalBytes.AddRange(saveBytes);

				return totalBytes.ToArray();
			}
		}
	}

}
