#region Info
// -----------------------------------------------------------------------
// V_IHasWorldPosition.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

namespace V_ObjectSystem
{

	public class V_IHasWorldPosition_Class : V_IHasWorldPosition
	{

		private readonly Func<Vector3> GetPositionFunc;

		public V_IHasWorldPosition_Class(Func<Vector3> GetPositionFunc)
		{
			this.GetPositionFunc = GetPositionFunc;
		}
		public Vector3 GetPosition()
		{
			return GetPositionFunc();
		}
	}
	public interface V_IHasWorldPosition
	{

		Vector3 GetPosition();
	}

}
