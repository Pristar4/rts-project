#region Info
// -----------------------------------------------------------------------
// V_IObjectTransform.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion

namespace V_ObjectSystem
{

	public interface V_IObjectTransform
	{

		void SetPosition(Vector3 position);
		Vector3 GetPosition();

		void SetScale(Vector3 scale);
		Vector3 GetScale();

		void SetEuler(Vector3 euler);
		Vector3 GetEuler();

		void SetEulerZ(float eulerZ);
		float GetEulerZ();

		Transform GetTransform();
	}

}
