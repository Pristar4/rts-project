#region Info
// -----------------------------------------------------------------------
// V_Animation.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#define SILENT

#region
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;
#endregion

namespace V_AnimationSystem
{

	public static class V_Animation
	{
		public delegate string ConvertTo_String<T>(T item);






		public delegate T ConvertTo_T<T>(string save);
		public enum DataLocation
		{
			Assets,
			Resources
		}
		public const string version = "1.00";
		public const string versionDate = "12-05-2018";

		public const DataLocation DATA_LOCATION = DataLocation.Resources;

		public const string fileExtention_AnimationType = "hkt";
		public const string fileExtention_Animation = "hka";

		private static readonly string[] prevVersions = { "0.01" };
		public static readonly byte versionByte = (byte)prevVersions.Length;

		public static readonly string LOC_ANIMATIONS
				= Application.dataPath + @"/Data/Animations/";
		public static readonly string LOC_ANIMATIONTYPES
				= Application.dataPath + @"/Data/AnimationTypes/";
		public static readonly string LOC_DEFAULT_ANIMATIONS
				= Application.dataPath + @"/Data/_Default/Animations/";
		public static readonly string LOC_DEFAULT_ANIMATIONTYPES
				= Application.dataPath + @"/Data/_Default/AnimationTypes/";

		private static bool isInit;


		private static void Init_Folders()
		{
			if (!Directory.Exists(LOC_ANIMATIONS))
				Directory.CreateDirectory(LOC_ANIMATIONS);
			if (!Directory.Exists(LOC_ANIMATIONTYPES))
				Directory.CreateDirectory(LOC_ANIMATIONTYPES);
			if (!Directory.Exists(LOC_DEFAULT_ANIMATIONS))
				Directory.CreateDirectory(LOC_DEFAULT_ANIMATIONS);
			if (!Directory.Exists(LOC_DEFAULT_ANIMATIONTYPES))
				Directory.CreateDirectory(LOC_DEFAULT_ANIMATIONTYPES);
		}
		public static void Init()
		{
			if (isInit) return;
			isInit = true;
#if !SILENT
            Debug.Log("V_Animation.Init "+DATA_LOCATION);
#endif

			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

			if (DATA_LOCATION == DataLocation.Assets) Init_Folders();

			UVType.Init();
			UnitAnim.Init();
			UnitAnimType.Init();
		}


		// Utilities
		public static T GetEnumFromString<T>(string convert)
		{
			foreach (T en in Enum.GetValues(typeof(T)))
				if (en.ToString() == convert)
					return en;
			return default;
		}
		public static float Parse_Float(string txt, float _default)
		{
			float f;
			if (!float.TryParse(txt, out f)) f = _default;
			return f;
		}
		public static int Parse_Int(string txt, int _default)
		{
			int i;
			if (!int.TryParse(txt, out i)) i = _default;
			return i;
		}
		public static int Parse_Int(string txt)
		{
			return Parse_Int(txt, -1);
		}
		public static string[] SplitString(string save, string separator)
		{
			return save.Split(new[] { separator }, StringSplitOptions.None);
		}
		public static void StringArrPush(ref string[] arr, string value)
		{
			var ret = new string[arr.Length + 1];
			for (var i = 0; i < ret.Length - 1; i++) ret[i] = arr[i];
			ret[ret.Length - 1] = value;
			arr = ret;
		}
		public static void StringArrPushIfIndex(int index, ref string[] arr,
		                                        string value)
		{
			if (index >= arr.Length) StringArrPush(ref arr, value);
		}

		public static string Save_Vector3Rounded(Vector3 vec)
		{
			return Mathf.Round(vec.x) + "," + Mathf.Round(vec.y) + "," +
			       Mathf.Round(vec.z);
		}
		public static Vector3 Load_Vector3Rounded(string save)
		{
			var content = save.Split(new[] { "," }, StringSplitOptions.None);
			return new Vector3(
					float.Parse(content[0]),
					float.Parse(content[1]),
					float.Parse(content[2])
			);
		}
		public static string Save_Vector3(Vector3 vec)
		{
			return Mathf.Round(vec.x * 1000f) + "," +
			       Mathf.Round(vec.y * 1000f) + "," +
			       Mathf.Round(vec.z * 1000f);
		}
		public static Vector3 Load_Vector3(string save)
		{
			var content = save.Split(new[] { "," }, StringSplitOptions.None);
			return new Vector3(
					float.Parse(content[0]) / 1000f,
					float.Parse(content[1]) / 1000f,
					float.Parse(content[2]) / 1000f
			);
		}
		public static string Save_Vector2(Vector2 vec, float multiplier = 1000f)
		{
			return Mathf.Round(vec.x * multiplier) + "," +
			       Mathf.Round(vec.y * multiplier);
		}
		public static Vector3 Load_Vector2(string save,
		                                   float multiplier = 1000f)
		{
			var content = save.Split(new[] { "," }, StringSplitOptions.None);
			return new Vector2(
					float.Parse(content[0]) / multiplier,
					float.Parse(content[1]) / multiplier
			);
		}

		public static string Save_List<T>(List<T> list,
		                                  ConvertTo_String<T> convertFunc,
		                                  string separator = "#LIST#")
		{
			if (list == null) return "";
			var ret = "";
			foreach (var single in list) ret += convertFunc(single) + separator;
			return ret;
		}
		public static List<T> Load_List<T>(string save,
		                                   ConvertTo_T<T> convertFunc,
		                                   string separator = "#LIST#")
		{
			var ret = new List<T>();
			var content
					= save.Split(new[] { separator }, StringSplitOptions.None);
			for (var i = 0; i < content.Length - 1; i++)
			{
				//Ignore last one
				var single = content[i];
				ret.Add(convertFunc(single));
			}
			return ret;
		}

		public static string Save_Array<T>(T[] array,
		                                   ConvertTo_String<T> convertFunc,
		                                   string separator = "#ARRAY#")
		{
			if (array == null) array = new T[0];
			var list = new List<T>();
			foreach (var t in array) list.Add(t);
			return Save_List(list, convertFunc, separator);
		}
		public static T[] Load_Array<T>(string save, ConvertTo_T<T> convertFunc,
		                                string separator = "#ARRAY#")
		{
			var array = Load_List(save, convertFunc, separator);
			return array.ToArray();
		}









		public static V_Skeleton_Anim[] V_LoadAnimationString(
				string readAllText, int animationFrameMultiplier = 2)
		{
			var content = SplitString(readAllText, "#ANIMATION#");
			var animKeyframes = new List<V_Skeleton_Anim>();
			animKeyframes = Load_List(content[0], V_Skeleton_Anim.Load,
					"#SKELETONANIMLIST#");

			// Duplicate frameCount
			foreach (var anim in animKeyframes)
			{
				foreach (var frame in anim.frames)
					frame.frameCount
							= frame.frameCount *
							  animationFrameMultiplier; // Increase frameCount
				anim.SetFrameRateOriginal(anim.GetFrameRateOriginal() /
				                          animationFrameMultiplier);
			}

			// Remake Tweens
			foreach (var anim in animKeyframes)
				anim.RemakeTween();

			return animKeyframes.ToArray();
		}
	}

}
