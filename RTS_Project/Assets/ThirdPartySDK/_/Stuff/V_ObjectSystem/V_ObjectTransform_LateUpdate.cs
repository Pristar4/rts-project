#region Info
// -----------------------------------------------------------------------
// V_ObjectTransform_LateUpdate.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
using Object = UnityEngine.Object;
#endregion

namespace V_ObjectSystem
{

	// Reference to Unity Transform, auto updates position on LateUpdate
	public class
			V_ObjectTransform_LateUpdate : V_IObjectTransform, V_IDestroySelf
	{

		private readonly GameObject gameObject;
		private readonly Func<Vector3> GetPositionFunc;
		private readonly Action OnDestroySelf;
		private readonly Transform transform;

		public V_ObjectTransform_LateUpdate(Transform transform,
		                                    Func<Vector3> GetPositionFunc,
		                                    V_Main.DelRegisterOnLateUpdate
				                                    RegisterOnLateUpdate,
		                                    V_Main.DelRegisterOnLateUpdate
				                                    DeregisterOnLateUpdate)
		{
			this.transform = transform;
			this.GetPositionFunc = GetPositionFunc;
			gameObject = transform.gameObject;

			RegisterOnLateUpdate(LateUpdate, V_Main.UpdateType.Main);

			OnDestroySelf = ()
					=> DeregisterOnLateUpdate(LateUpdate,
							V_Main.UpdateType.Main);
		}



		public void DestroySelf()
		{
			if (OnDestroySelf != null) OnDestroySelf();
			Object.Destroy(gameObject);
		}

		public void SetPosition(Vector3 position)
		{
			transform.position = position;
		}
		public Vector3 GetPosition()
		{
			return transform.position;
		}
		public void SetScale(Vector3 scale)
		{
			transform.localScale = scale;
		}
		public Vector3 GetScale()
		{
			return transform.localScale;
		}
		public void SetEuler(Vector3 euler)
		{
			transform.localEulerAngles = euler;
		}
		public Vector3 GetEuler()
		{
			return transform.localEulerAngles;
		}
		public void SetEulerZ(float eulerZ)
		{
			var euler = transform.localEulerAngles;
			euler.z = eulerZ;
			transform.localEulerAngles = euler;
		}
		public float GetEulerZ()
		{
			return transform.localEulerAngles.z;
		}


		public Transform GetTransform()
		{
			return transform;
		}
		private void LateUpdate(float deltaTime)
		{
			SetPosition(GetPositionFunc());
		}
	}


}
