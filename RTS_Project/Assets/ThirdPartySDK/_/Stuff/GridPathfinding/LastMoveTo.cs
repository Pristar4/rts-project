#region Info
// -----------------------------------------------------------------------
// LastMoveTo.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
#endregion
//using myNameSpace;

namespace GridPathfindingSystem
{

	public class LastMoveTo
	{
		public UnitMovement.PathCallback callback;
		public GridPathfinding.UnitMovementCallbackType callbackType;

		public List<MapPos> mapPos;
		public object obj;

		public LastMoveTo(List<MapPos> _mapPos,
		                  GridPathfinding.UnitMovementCallbackType
				                  _callbackType,
		                  object _obj, UnitMovement.PathCallback _callback)
		{
			mapPos = _mapPos;
			callbackType = _callbackType;
			obj = _obj;
			callback = _callback;
		}
		public LastMoveTo(MapPos _mapPos,
		                  GridPathfinding.UnitMovementCallbackType
				                  _callbackType,
		                  object _obj, UnitMovement.PathCallback _callback)
		{
			mapPos = new List<MapPos> { _mapPos };
			callbackType = _callbackType;
			obj = _obj;
			callback = _callback;
		}
	}

}
