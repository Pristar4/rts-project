#region Info
// -----------------------------------------------------------------------
// V_Main.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace V_ObjectSystem
{

	public static class V_Main
	{



		public delegate void DelRegisterOnLateUpdate(
				DelUpdate onLateUpdate, UpdateType updateType);




		public delegate void DelRegisterOnUpdate(DelUpdate onUpdate,
		                                         UpdateType updateType);

		public delegate void DelUpdate(float deltaTime);

		public enum UpdateType
		{
			Main,
			Unit,
			Camera,
			Unpaused
		}

		private static Dictionary<UpdateType, float>
				updateTimeModDic; // Update Time Modifier
		private static Dictionary<UpdateType, List<DelUpdate>>
				onUpdateListTypeDic; // Update Action List
		private static Dictionary<UpdateType, List<DelUpdate>>
				onLateUpdateListTypeDic; // Late Update Action List

		private static MonoBehaviourHook monoBehaviourHook;
		private static UpdateType[] updateTypeArr;

		public static void Init()
		{
			if (monoBehaviourHook != null) // Already initialized
				return;
			V_TimeScaleManager.Init();

			updateTypeArr = (UpdateType[])Enum.GetValues(typeof(UpdateType));

			updateTimeModDic = new Dictionary<UpdateType, float>();

			onUpdateListTypeDic = new Dictionary<UpdateType, List<DelUpdate>>();
			onLateUpdateListTypeDic
					= new Dictionary<UpdateType, List<DelUpdate>>();

			foreach (var updateType in updateTypeArr)
			{
				updateTimeModDic[updateType] = 1f;

				onUpdateListTypeDic[updateType] = new List<DelUpdate>();
				onLateUpdateListTypeDic[updateType] = new List<DelUpdate>();
			}

			var gameObject = new GameObject("V_Main_GameObject");
			monoBehaviourHook = gameObject.AddComponent<MonoBehaviourHook>();
			monoBehaviourHook.OnUpdate = Update;
		}
		public static void SetUpdateTimeMod(UpdateType updateType, float mod)
		{
			if (updateTimeModDic == null) return;
			updateTimeModDic[updateType] = mod;
		}

		// Manually trigger Update
		public static void TriggerFakeUpdate(float deltaTime)
		{
			Update(deltaTime);
		}

		private static void Update()
		{
			Update(Time.deltaTime);
		}
		private static void Update(float deltaTime)
		{
			foreach (var updateType in updateTypeArr)
			{
				var tmpOnUpdateList
						= new List<DelUpdate>(onUpdateListTypeDic[updateType]);
				for (var i = 0; i < tmpOnUpdateList.Count; i++)
					tmpOnUpdateList
							[i](deltaTime * updateTimeModDic[updateType]);
			}

			// Update V_Object's
			V_Object.Static_Update(deltaTime, updateTimeModDic);

			foreach (var updateType in updateTypeArr)
			{
				var tmpOnLateUpdateList
						= new List<DelUpdate>(
								onLateUpdateListTypeDic[updateType]);
				for (var i = 0; i < tmpOnLateUpdateList.Count; i++)
					tmpOnLateUpdateList[i](deltaTime *
					                       updateTimeModDic[updateType]);
			}
		}

		public static void RegisterOnUpdate(DelUpdate onUpdate,
		                                    UpdateType updateType
				                                    = UpdateType.Main)
		{
			Init();
			onUpdateListTypeDic[updateType].Add(onUpdate);
		}
		public static void DeregisterOnUpdate(DelUpdate onUpdate,
		                                      UpdateType updateType
				                                      = UpdateType.Main)
		{
			onUpdateListTypeDic[updateType].Remove(onUpdate);
		}


		public static void RegisterOnLateUpdate(DelUpdate onLateUpdate,
		                                        UpdateType updateType
				                                        = UpdateType.Main)
		{
			Init();
			onLateUpdateListTypeDic[updateType].Add(onLateUpdate);
		}
		public static void DeregisterOnLateUpdate(DelUpdate onLateUpdate,
		                                          UpdateType updateType
				                                          = UpdateType.Main)
		{
			onLateUpdateListTypeDic[updateType].Remove(onLateUpdate);
		}

		/*
		 * Class to hook Actions into MonoBehaviour
		 * */
		private class MonoBehaviourHook : MonoBehaviour
		{

			public Action OnUpdate;

			private void Update()
			{
				OnUpdate();
			}
		}
	}

}
