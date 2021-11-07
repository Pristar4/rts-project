#region Info
// -----------------------------------------------------------------------
// V_Object.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.MonoBehaviours;
using UnityEngine;
using V_AnimationSystem;
using Object = UnityEngine.Object;
#endregion

namespace V_ObjectSystem
{

	public class V_Object
	{

		private static readonly List<V_Object> instanceList
				= new List<V_Object>();

		private static readonly Dictionary<Type, List<V_Object>> typeInstanceDic
				= new Dictionary<Type, List<V_Object>>();


		private static int nextId;

		private readonly List<V_IObjectActiveLogic> allLogicList;
		private readonly Dictionary<Type, object> logicDic;

		private readonly List<object> relatedObjectList;

		private V_IObjectActiveLogic[] activeLogicArr;
		private V_IObjectActiveLogic[] activeLogicArrCopy;

		private float deltaTimeModifier;

		// Helper accessor
		public Func<Vector3> GetPosition;
		public Func<Transform> GetTransform;

		public int id;



		public bool isDestroyed;

		private bool isDisabled;

		private Dictionary<Type, List<object>> logicListDic;

		// Mandatory
		private string name;

		public bool TESTER = false;

		private V_Main.UpdateType updateType;

		private V_Object(V_Main.UpdateType updateType)
		{
			V_Main.Init();
			instanceList.Add(this);
			id = GetNextId();

			this.updateType = updateType;

			isDestroyed = false;
			isDisabled = false;

			activeLogicArr = new V_IObjectActiveLogic[0];
			activeLogicArrCopy = new V_IObjectActiveLogic[0];

			allLogicList = new List<V_IObjectActiveLogic>();

			relatedObjectList = new List<object>();

			logicListDic = new Dictionary<Type, List<object>>();
			logicDic = new Dictionary<Type, object>();

			GetPosition = delegate { return Vector3.zero; };
			GetTransform = delegate { return null; };

			deltaTimeModifier = V_TimeScaleManager.GetTimeScale();
		}
		private static int GetNextId()
		{
			return nextId++;
		}

		public event EventHandler OnDestroySelf;
		public void SetIsDisabled(bool isDisabled)
		{
			this.isDisabled = isDisabled;
		}
		public float GetDeltaTimeModifier()
		{
			return deltaTimeModifier;
		}
		public void SetDeltaTimeModifier(float f)
		{
			deltaTimeModifier = f;
		}
		public V_Main.UpdateType GetUpdateType()
		{
			return updateType;
		}
		public void SetUpdateType(V_Main.UpdateType updateType)
		{
			this.updateType = updateType;
		}
		public void AddActiveLogic(V_IObjectActiveLogic activeLogic)
		{
			var tmpLogicList = new List<V_IObjectActiveLogic>(activeLogicArr);
			tmpLogicList.Add(activeLogic);
			activeLogicArr = tmpLogicList.ToArray();
			activeLogicArrCopy = tmpLogicList.ToArray();

			allLogicList.Add(activeLogic);

			logicDic[activeLogic.GetType()] = activeLogic;
		}
		public void RemoveActiveLogic(V_IObjectActiveLogic activeLogic)
		{
			var tmpLogicList = new List<V_IObjectActiveLogic>(activeLogicArr);
			tmpLogicList.Remove(activeLogic);
			activeLogicArr = tmpLogicList.ToArray();
			activeLogicArrCopy = tmpLogicList.ToArray();

			allLogicList.Remove(activeLogic);
			logicDic[activeLogic.GetType()] = null;
		}
		public void AddRelatedObject(object obj)
		{
			if (typeof(V_IHasWorldPosition).IsAssignableFrom(obj.GetType()))
				GetPosition = (obj as V_IHasWorldPosition).GetPosition;
			if (typeof(V_IObjectTransform).IsAssignableFrom(obj.GetType()))
				GetTransform = (obj as V_IObjectTransform).GetTransform;

			relatedObjectList.Add(obj);
			logicDic[obj.GetType()] = obj;
		}
		public void RemoveRelatedObject(object obj)
		{
			if (typeof(V_IHasWorldPosition).IsAssignableFrom(obj.GetType()))
				if (GetPosition == (obj as V_IHasWorldPosition).GetPosition)
					GetPosition = delegate { return Vector3.zero; };
			if (typeof(V_IObjectTransform).IsAssignableFrom(obj.GetType()))
				if (GetTransform == (obj as V_IObjectTransform).GetTransform)
					GetTransform = delegate { return null; };

			relatedObjectList.Remove(obj);
			logicDic[obj.GetType()] = null;
		}


		// Caled every frame from V_Main
		public static void Static_Update(float deltaTime,
		                                 Dictionary<V_Main.UpdateType, float>
				                                 updateTimeModDic)
		{
			var tmpInstanceList = new List<V_Object>(instanceList);
			for (var i = 0; i < tmpInstanceList.Count; i++)
				tmpInstanceList[i]
						.Update(deltaTime *
						        updateTimeModDic[
								        tmpInstanceList[i].updateType]);
		}
		public static V_Object GetObject(string name)
		{
			foreach (var vObject in instanceList)
				if (vObject.name == name)
					return vObject;
			return null;
		}
		private void Update(float deltaTime)
		{
			if (isDestroyed) return;
			if (isDisabled) return;

			deltaTime = deltaTime * deltaTimeModifier;

			// Execute all logic
			for (var i = 0; i < activeLogicArrCopy.Length; i++)
			{
				activeLogicArrCopy[i].Update(deltaTime);
				if (isDestroyed) return;
			}
		}


		public bool HasLogic<T>()
		{
			return GetLogic<T>() != null;
		}

		public bool HasAllLogic<T1, T2>()
		{
			if (!HasLogic<T1>()) return false;
			if (!HasLogic<T2>()) return false;
			return true;
		}

		public bool HasAllLogic<T1, T2, T3>()
		{
			if (!HasLogic<T1>()) return false;
			if (!HasLogic<T2>()) return false;
			if (!HasLogic<T3>()) return false;
			return true;
		}

		public bool TryGetLogic<T>(out T t)
		{
			t = GetLogic<T>();
			return t != null;
		}

		public T GetLogic<T>()
		{
			object objectInstance;
			if (logicDic.TryGetValue(typeof(T), out objectInstance))
				return (T)objectInstance;

			for (var i = 0; i < allLogicList.Count; i++)
				if (typeof(T).IsAssignableFrom(allLogicList[i].GetType()))
				{
					logicDic[typeof(T)] = allLogicList[i];
					return (T)allLogicList[i];
				}
			for (var i = 0; i < relatedObjectList.Count; i++)
				if (typeof(T).IsAssignableFrom(relatedObjectList[i].GetType()))
				{
					logicDic[typeof(T)] = relatedObjectList[i];
					return (T)relatedObjectList[i];
				}

			return default;
		}
		public void GetLogic<T>(out T ret)
		{
			ret = GetLogic<T>();
		}
		public void DestroySelf()
		{
			if (isDestroyed) return;

			isDestroyed = true;
			instanceList.Remove(this);
			foreach (var type in typeInstanceDic.Keys)
				typeInstanceDic[type].Remove(this);

			// Clean up related objects
			for (var i = 0; i < allLogicList.Count; i++)
				if (typeof(V_IDestroySelf).IsAssignableFrom(allLogicList[i]
						.GetType()))
					(allLogicList[i] as V_IDestroySelf).DestroySelf();
			for (var i = 0; i < relatedObjectList.Count; i++)
				if (typeof(V_IDestroySelf).IsAssignableFrom(relatedObjectList[i]
						.GetType()))
					(relatedObjectList[i] as V_IDestroySelf).DestroySelf();

			if (OnDestroySelf != null) OnDestroySelf(this, EventArgs.Empty);
		}





		public static void TryDestroyObject(string name)
		{
			var vObject = GetObject(name);
			if (vObject != null) vObject.DestroySelf();
		}











		/*
		public V_ObjectLogic_DebugText AddDebugText(Func<string> GetDebugString) {
		    V_ObjectLogic_DebugText debugText = new V_ObjectLogic_DebugText(GetPosition, GetDebugString);
		    this.AddActiveLogic(debugText);
		    return debugText;
		}
		public static V_Object CreateDebugText_UI(Func<string> GetDebugString) {
		    return CreateDebugText_UI(Vector3.zero, GetDebugString);
		}
		public static V_Object CreateDebugText_UI(Vector3 offsetPosition, Func<string> GetDebugString) {
		    return CreateDebugText(() => {
		        return UICanvas.uiCornerLL.position + offsetPosition;
		    }, GetDebugString);
		}
		public static V_Object CreateDebugTextPopup(string text, Vector3 position) {
		    float startTime = Time.realtimeSinceStartup;
		    float popupTime = 2f;
		    Vector3 popupOffset = new Vector3(0, 10);
		    Func<Vector3> GetPosition = delegate () {
		        float timeMultiplier = (Time.realtimeSinceStartup - startTime) / popupTime;
		        return position + popupOffset * timeMultiplier;
		    };
		    V_Object vObject = CreateDebugText(GetPosition, () => text);
		    FunctionTimer.Create(vObject.DestroySelf, popupTime);
		    return vObject;
		}
		public static V_Object CreateDebugText(Vector3 position, Func<string> GetDebugString) {
		    return CreateDebugText(() => { return position; }, GetDebugString);
		}
		public static V_Object CreateDebugText(Func<Vector3> GetPosition, Func<string> GetDebugString) {
		    V_Object debugObject = V_Object.CreateObject();
		    debugObject.AddActiveLogic(new V_ObjectLogic_DebugText(GetPosition, GetDebugString));
		    return debugObject;
		}
		public static V_Object CreateDebugUpdater(Action<float> action) {
		    V_Object debugObject = V_Object.CreateObject();
		    debugObject.AddActiveLogic(new V_ObjectLogic_Custom(delegate (object sender, V_ObjectLogic_Custom.OnUpdateEventArgs e) { action(e.deltaTime); }, true, true, true));
		    return debugObject;
		}
		*/









		public static V_Object CreateObject(
				V_Main.UpdateType updateType = V_Main.UpdateType.Main)
		{
			return new V_Object(updateType);
		}
		public static V_Object CreateObject(string name,
		                                    V_Main.UpdateType updateType
				                                    = V_Main.UpdateType.Main)
		{
			var vObject = CreateObject(updateType);
			vObject.name = name;
			return vObject;
		}
		public static V_Object CreateObjectPrefab(GameObject prefab,
		                                          Vector3 position)
		{
			var instancedObject = CreateObject();
			var instancedTransform = Object
					.Instantiate(prefab, position, Quaternion.identity)
					.transform;
			V_IObjectTransform instanceTransform
					= new V_ObjectTransform(instancedTransform);
			instancedObject.AddRelatedObject(instanceTransform);
			instancedObject.AddRelatedObject(
					new V_IHasWorldPosition_Class(instanceTransform
							.GetPosition));
			return instancedObject;
		}
		public static V_Object CreateObjectGameObject(GameObject gameObject)
		{
			var instancedObject = CreateObject();
			var instancedTransform = gameObject.transform;
			V_IObjectTransform instanceTransform
					= new V_ObjectTransform(instancedTransform);
			instancedObject.AddRelatedObject(instanceTransform);
			instancedObject.AddRelatedObject(
					new V_IHasWorldPosition_Class(instanceTransform
							.GetPosition));
			return instancedObject;
		}
		public static V_Object CreateSkeleton(Vector3 position,
		                                      Material material)
		{
			var instancedObject = CreateObject();
			var gameObject = new GameObject("Skeleton", typeof(MeshFilter),
					typeof(MeshRenderer));
			var instantiatedTransform = gameObject.transform;
			instantiatedTransform.position = position;
			//Transform instantiatedTransform = UnityEngine.Object.Instantiate(SpriteHolder.instance.t_pfV_Skeleton, position, Quaternion.identity);
			V_IObjectTransform transform
					= new V_ObjectTransform(instantiatedTransform);
			instancedObject.AddRelatedObject(transform);
			V_IObjectTransformBody transformBody
					= new V_ObjectTransformBody(instantiatedTransform,
							material);
			instancedObject.AddRelatedObject(transformBody);
			instancedObject.AddRelatedObject(
					new V_IHasWorldPosition_Class(transform.GetPosition));

			var unitSkeleton = new V_UnitSkeleton(1f,
					transformBody.ConvertBodyLocalPositionToWorldPosition,
					transformBody.SetBodyMesh);
			instancedObject.AddRelatedObject(unitSkeleton);
			instancedObject.AddActiveLogic(
					new V_ObjectLogic_SkeletonUpdater(unitSkeleton));
			var unitAnimation = new V_UnitAnimation(unitSkeleton);
			instancedObject.AddRelatedObject(unitAnimation);

			return instancedObject;
		}
		public static V_Object CreateSkeletonPresetAnim(
				Vector3 position, Material material, UnitAnimType unitAnimType,
				Vector3 dir, float frameRateMod)
		{
			var instancedObject = CreateSkeleton(position, material);
			V_UnitSkeleton.OnAnimComplete onAnimComplete = delegate
			{
				instancedObject.DestroySelf();
			};
			instancedObject.GetLogic<V_UnitAnimation>().PlayAnimForced(
					unitAnimType,
					dir, frameRateMod, onAnimComplete, null, null);
			return instancedObject;
		}
		public static V_Object CreateSkeletonPresetAnim(
				Vector3 position, Material material, UnitAnimType unitAnimType,
				Vector3 dir, float frameRateMod, Vector3 rotatePrefabDir)
		{
			var instancedObject = CreateSkeleton(position, material);
			V_UnitSkeleton.OnAnimComplete onAnimComplete = delegate
			{
				instancedObject.DestroySelf();
			};
			instancedObject.GetLogic<V_UnitAnimation>().PlayAnimForced(
					unitAnimType,
					dir, frameRateMod, onAnimComplete, null, null);
			instancedObject.GetLogic<V_IObjectTransform>()
					.SetEulerZ(GetAngleFromVector(rotatePrefabDir));
			return instancedObject;
		}
		public static V_Object CreateSkeletonPresetAnim(
				Vector3 position, Material material, UnitAnimType unitAnimType,
				Vector3 dir, float frameRateMod, Vector3 rotatePrefabDir,
				Vector3 scalePrefab)
		{
			var instancedObject = CreateSkeleton(position, material);
			V_UnitSkeleton.OnAnimComplete onAnimComplete = delegate
			{
				instancedObject.DestroySelf();
			};
			instancedObject.GetLogic<V_UnitAnimation>().PlayAnimForced(
					unitAnimType,
					dir, frameRateMod, onAnimComplete, null, null);
			instancedObject.GetLogic<V_IObjectTransform>()
					.SetEulerZ(GetAngleFromVector(rotatePrefabDir));
			instancedObject.GetLogic<V_IObjectTransform>()
					.SetScale(scalePrefab);
			return instancedObject;
		}
		public static V_Object CreateSkeletonPresetAnim(
				Vector3 position, Material material, UnitAnim unitAnim,
				float frameRateMod, Vector3 rotatePrefabDir,
				Vector3 scalePrefab,
				int sortingOrderOffset)
		{
			var instancedObject = CreateSkeleton(position, material);
			V_UnitSkeleton.OnAnimComplete onAnimComplete = delegate
			{
				instancedObject.DestroySelf();
			};
			instancedObject.GetLogic<V_IObjectTransform>().GetTransform()
					.GetComponent<PositionRendererSorter>()
					.SetOffset(sortingOrderOffset);
			instancedObject.GetLogic<V_UnitAnimation>().PlayAnimForced(unitAnim,
					frameRateMod, onAnimComplete, null, null);
			instancedObject.GetLogic<V_IObjectTransform>()
					.SetEulerZ(GetAngleFromVector(rotatePrefabDir));
			instancedObject.GetLogic<V_IObjectTransform>()
					.SetScale(scalePrefab);
			return instancedObject;
		}
		public static V_Object CreateSkeletonPresetAnim_DebugDash(
				Vector3 position, Vector3 rotationDir, Vector3 localScale)
		{
			var vObject = CreateSkeletonPresetAnim(position,
					GetWhitePixelMaterial(),
					UnitAnim.GetUnitAnim("EffectDash"), 1f, rotationDir,
					localScale, 100);
			return vObject;
		}
		public static V_Object
				CreateSkeletonPresetAnim_DebugDash(Vector3 position)
		{
			var vObject = CreateSkeletonPresetAnim(position,
					GetWhitePixelMaterial(),
					UnitAnim.GetUnitAnim("EffectDash"), 1f,
					new Vector3(0, 1, 0),
					new Vector3(3f, 1f, 1f), 100);
			return vObject;
		}
		public static V_Object CreateSkeletonPresetAnim_DebugDash(
				Vector3 position, Color color, float frameRate = 1f)
		{
			var material = new Material(GetWhitePixelMaterial());
			material.color = color;
			var vObject = CreateSkeletonPresetAnim(position, material,
					UnitAnim.GetUnitAnim("EffectDash"), frameRate,
					new Vector3(0, 1, 0),
					new Vector3(3f, 1f, 1f), 100);
			return vObject;
		}





		public static List<V_Object> GetObjectsWithAllTheseLogicTypes<T>()
		{
			List<V_Object> filteredList;
			if (typeInstanceDic.TryGetValue(typeof(T), out filteredList))
			{
				// return cache
			}
			else
			{
				filteredList
						= FilterObjectsWithTheseLogicTypes<T>(instanceList);
				typeInstanceDic[typeof(T)] = filteredList;
			}
			return filteredList;
		}

		public static List<V_Object> FilterObjectsWithTheseLogicTypes<T>(
				List<V_Object> objectList)
		{
			var ret = new List<V_Object>();
			for (var i = 0; i < objectList.Count; i++)
			{
				var objectInstance = objectList[i];
				var logic = objectInstance.GetLogic<T>();
				if (logic != null) // Contains logic
					ret.Add(objectInstance);
			}
			return ret;
		}


		public static List<V_Object> FilterObjectList(
				List<V_Object> objectList,
				Func<V_Object, bool> ValidateObjectFunc)
		{
			for (var i = 0; i < objectList.Count; i++)
				if (!ValidateObjectFunc(objectList[i]))
				{
					objectList.RemoveAt(i);
					i--;
				}
			return objectList;
		}

		public static List<V_Object> FilterObjectList_Distance(
				List<V_Object> objectList, Vector3 position, float distance)
		{
			// Filter objects within distance
			return FilterObjectList(objectList,
					GetValidateObjectFuncTestDistance(position, distance));
		}




		public static V_Object GetClosestFromList(Vector3 position,
		                                          List<V_Object> objectList)
		{
			V_Object closest = null;
			for (var i = 0; i < objectList.Count; i++)
				if (closest == null) { closest = objectList[i]; }
				else
				{
					// No closest
					if (Vector3.Distance(position,
							    objectList[i].GetPosition()) <
					    Vector3.Distance(position, closest.GetPosition()))
						closest = objectList[i];
				}
			return closest;
		}
		public static Func<V_Object, bool> GetValidateObjectFuncTestDistance(
				Vector3 position, float distance)
		{
			return delegate(V_Object vObject)
			{
				return Vector3.Distance(position, vObject.GetPosition()) <=
				       distance;
			};
		}



		public static void SetAllDeltaTimeModifier(float deltaTimeModifier)
		{
			foreach (var singleObject in instanceList)
				singleObject.SetDeltaTimeModifier(deltaTimeModifier);
		}


		private static int GetAngleFromVector(Vector3 dir)
		{
			dir = dir.normalized;
			var n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			if (n < 0) n += 360;
			var angle = Mathf.RoundToInt(n);

			return angle;
		}
		private static Material GetWhitePixelMaterial()
		{
			return Assets.i.m_White;
		}


		// Destroy all objects that have this particular logic
		public static void TriggerDestroySelfOnObjectsWithLogic<T>()
		{
			var tmpObjectList
					= new List<V_Object>(GetObjectsWithAllTheseLogicTypes<T>());
			foreach (var objectInstance in tmpObjectList)
				objectInstance.DestroySelf();
		}


		public static void CleanUpAllObjects()
		{
			var tmpInstanceList = new List<V_Object>(instanceList);
			foreach (var objectInstance in tmpInstanceList)
				objectInstance.DestroySelf();
		}
	}


}
