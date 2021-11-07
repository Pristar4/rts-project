#region Info
// -----------------------------------------------------------------------
// UnitMovement.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace GridPathfindingSystem
{

	public class UnitMovement
	{
		public delegate Vector3 DelGetPosition();
		public delegate void DelSetPosition(Vector3 set);
		public delegate void PathCallback(
				GridPathfinding.UnitMovementCallbackType cType, object obj);
		private GridPathfinding.UnitMovementCallbackType callbackType;
		private int curX = 7;
		private int curY;
		public bool destroyed;
		private MapPos finalPos;
		private DelGetPosition getPosition;
		private bool isMoving;
		private Vector3 lastDir = Vector3.zero;
		private LastMoveTo lastMoveTo;
		private MapPos mapPos;
		private object obj;
		private PathCallback pathCallback;

		private List<PathNode> pathList;
		private DelSetPosition setPosition;
		private float speed = 30f;

		//private int layerMask = 1 << 9;

		public void Setup(MapPos mapPos, DelGetPosition getPosition,
		                  DelSetPosition setPosition, float speed)
		{
			this.mapPos = mapPos;
			this.getPosition = getPosition;
			this.setPosition = setPosition;
			this.speed = speed;
		}
		// Update is called once per frame
		public void Update(float deltaTime)
		{
			if (pathList == null || pathList != null && pathList.Count <= 0)
				return;
			var pos = getPosition();
			isMoving = true;
			var currPath = pathList[0];
			Vector3 currPos;
			var distanceCheck = 2f;
			if (pathList.Count == 1)
			{ // Last Pos
				distanceCheck = 1f;
				currPos = new Vector3(currPath.xPos * 10 + finalPos.offsetX,
						currPath.yPos * 10 + finalPos.offsetY);
			}
			else
			{
				currPos = new Vector3(currPath.xPos * 10, currPath.yPos * 10);
			}

			// Move to path
			var dir = (currPos - pos).normalized;
			lastDir = dir;
			var moveAmount
					= Mathf.Min(deltaTime * speed,
							Vector3.Distance(pos, currPos));
			var dirAmt = dir * moveAmount;
			setPosition(new Vector3(pos.x + dirAmt.x, pos.y + dirAmt.y));
			pos = getPosition();

			if (pathList.Count > 1)
			{ //Not last pos
				var prevPos = new MapPos(mapPos.x, mapPos.y);
				if (mapPos.offsetX > 0 || mapPos.offsetY > 0)
				{
					// Last target had an offset
					if (Mathf.RoundToInt(pos.x / 10f) == mapPos.x &&
					    Mathf.RoundToInt(pos.y / 10f) == mapPos.y)
					{
						// Rounded position equals base MapPos without Offset
						mapPos.x = Mathf.RoundToInt(pos.x / 10f);
						mapPos.y = Mathf.RoundToInt(pos.y / 10f);
						mapPos.offsetX = 0f;
						mapPos.offsetY = 0f;
					}
				}
				else
				{
					mapPos.x = Mathf.RoundToInt(pos.x / 10f);
					mapPos.y = Mathf.RoundToInt(pos.y / 10f);
				}

				if (prevPos.x != mapPos.x || prevPos.y != mapPos.y)
				{
					//Event_Speaker.Broadcast(Event_Trigger.Unit_Moved, mapPos);
					//unit.OnUnitMoved(prevPos, mapPos);
				}
			}

			if (Vector3.Distance(pos, currPos) < distanceCheck)
			{
				// Next path
				pathList.RemoveAt(0);
				if (pathList.Count == 0)
				{
					// Final destination reached
					setPosition(currPos);
					pathList = null;

					mapPos.offsetX = finalPos.offsetX;
					mapPos.offsetY = finalPos.offsetY;
					mapPos.straightToOffset = finalPos.straightToOffset;
					if (finalPos.straightToOffset)
					{
						mapPos.x = finalPos.x;
						mapPos.y = finalPos.y;
					}
					PathComplete();
				}
			}
		}
		public Vector3 GetLastDir()
		{
			return lastDir;
		}
		public void SetSpeed(float spd)
		{
			speed = spd;
		}
		public void PathComplete()
		{
			if (destroyed) return;
			isMoving = false;
			if (pathCallback != null) pathCallback(callbackType, obj);
		}
		public void OnPathComplete(List<PathNode> path, MapPos _mapPos)
		{
			if (destroyed) return;
			var previousFinalPos = finalPos;
			finalPos = _mapPos;

			//Debug.Log("Pathfind - "+finalPos);
			//Optimize path, look for direct shortcuts.

			/*
			RaycastHit hit;
			for (int i=0; i<path.Count-2; i++) {
			    Vector3 pos = new Vector3(path[i].xPos*10,0,path[i].zPos*10);
			    //If this is on top of a hitbox, don't shortcut
			    if (Physics.Raycast(new Vector3(pos.x, 1, pos.z), new Vector3(0,-1,0), out hit, 2, layerMask)) {
			        continue;
			    }
	  
			    for (int j=i+1; j<path.Count-1; j++) {
			        Vector3 pos2 = new Vector3(path[j].xPos*10,0,path[j].zPos*10);
			        if (!Physics.Raycast(pos, (pos2-pos).normalized, out hit, Vector3.Distance(pos,pos2), layerMask)) {
			            if (j > i+1) {
			                path.RemoveAt(j-1);
			                j--;
			            }
			        } else {
			            //Debug.Log("hit");
			            break;
			        }
			    }
			}
			*/

			// See if going to path[0] involves going backwards
			// Check if Distance(currentPos,path[1]) < Distance(path[0],path[1])
			if (path.Count > 1 && (previousFinalPos == null ||
			                       previousFinalPos.offsetX == 0 &&
			                       previousFinalPos.offsetY == 0))
				if (Vector3.Distance(getPosition(),
						    new Vector3(path[1].xPos * 10, path[1].yPos * 10)) <
				    Vector3.Distance(
						    new Vector3(path[0].xPos * 10, path[0].yPos * 10),
						    new Vector3(path[1].xPos * 10,
								    path[1].yPos *
								    10))) //Currently closer, skip first position
					path.RemoveAt(0);
			pathList = path;
		}

		public bool MoveTo(MapPos _mapPos,
		                   GridPathfinding.UnitMovementCallbackType
				                   _callbackType
				                   = GridPathfinding.UnitMovementCallbackType
						                   .Simple,
		                   object _obj = null, PathCallback callback = null)
		{
			lastMoveTo = new LastMoveTo(_mapPos, _callbackType, _obj, callback);
			callbackType = _callbackType;
			obj = _obj;
			pathCallback = callback;
			curX = mapPos.x;
			curY = mapPos.y;

			//return MyPathfinding.FindPath(curX,curY,_mapPos,OnPathComplete);
			return false;
		}
		public bool MoveTo(List<MapPos> _mapPos,
		                   GridPathfinding.UnitMovementCallbackType
				                   _callbackType,
		                   object _obj, PathCallback callback)
		{
			lastMoveTo = new LastMoveTo(_mapPos, _callbackType, _obj, callback);
			callbackType = _callbackType;
			obj = _obj;
			pathCallback = callback;
			curX = mapPos.x;
			curY = mapPos.y;

			//return MyPathfinding.FindPath(curX,curY,_mapPos,OnPathComplete);
			return false;
		}
		public bool MoveTo(LastMoveTo _lastMoveTo)
		{
			return MoveTo(_lastMoveTo.mapPos, _lastMoveTo.callbackType,
					_lastMoveTo.obj, _lastMoveTo.callback);
		}
		public void DestroySelf()
		{
			destroyed = true;
		}
	}

}
