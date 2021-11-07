#region Info
// -----------------------------------------------------------------------
// PathNode.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

namespace GridPathfindingSystem
{

	public class PathNode
	{
		public PathNode east;
		public int fValue;
		public int gValue = 0;
		public int hValue;
		public bool isOnClosedList = false;

		public bool isOnOpenList = false;
		public bool moveEast;
		public bool moveNorth;
		public bool moveSouth;
		public bool moveWest;
		public PathNode north;
		public PathNode parent;
		public PathNode south;

		public int weight;
		public PathNode west;

		public int xPos;
		public int yPos;

		//public Transform trans;
		//public int layerMask = 1 << 9;

		public PathNode(int _xPos, int _yPos)
		{
			xPos = _xPos;
			yPos = _yPos;

			moveNorth = true;
			moveSouth = true;
			moveWest = true;
			moveEast = true;

			//trans = ((GameObject) Object.Instantiate(Resources.Load("pfPathNode"), new Vector3(xPos*10, 0, zPos*10), Quaternion.identity)).transform;
			TestHitbox();
		}

		public event EventHandler OnWalkableChanged;
		public void ResetRestrictions()
		{
			moveNorth = true;
			moveSouth = true;
			moveWest = true;
			moveEast = true;
		}
		public override string ToString()
		{
			return "x: " + xPos + ", y:" + yPos;
		}
		public void SetWalkable(bool walkable)
		{
			weight = walkable ? 0 : GridPathfinding.WALL_WEIGHT;
			if (OnWalkableChanged != null)
				OnWalkableChanged(this, EventArgs.Empty);
		}
		public void SetWeight(int weight)
		{
			this.weight = weight;
		}
		public bool IsWalkable()
		{
			return weight < GridPathfinding.WALL_WEIGHT;
		}
		public void TestHitbox()
		{
			weight = 0;
		}
		public MapPos GetMapPos()
		{
			return new MapPos(xPos, yPos);
		}
		public void CalculateFValue()
		{
			fValue = gValue + hValue;
		}
		public Vector3 GetWorldVector(Vector3 worldOrigin, float nodeSize)
		{
			return worldOrigin + new Vector3(xPos * nodeSize, yPos * nodeSize);
		}
	}

}
