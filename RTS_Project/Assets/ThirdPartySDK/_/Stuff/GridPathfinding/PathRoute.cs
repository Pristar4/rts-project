#region Info
// -----------------------------------------------------------------------
// PathRoute.cs
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

	public class PathRoute
	{
		public MapPos finalPos;

		public List<PathNode> pathNodeList;
		public List<Vector3> pathVectorList;

		public PathRoute(List<PathNode> pathNodeList,
		                 List<Vector3> pathVectorList,
		                 MapPos finalPos)
		{
			this.pathNodeList = pathNodeList;
			this.pathVectorList = pathVectorList;
			this.finalPos = finalPos;
		}

		public PathRoute(List<PathNode> pathNodeList, Vector3 worldOrigin,
		                 float nodeSize, MapPos finalPos)
		{
			this.pathNodeList = pathNodeList;
			pathVectorList = new List<Vector3>();
			foreach (var pathNode in pathNodeList)
				pathVectorList.Add(
						pathNode.GetWorldVector(worldOrigin, nodeSize));
			this.finalPos = finalPos;
		}

		public void AddVector(Vector3 vector)
		{
			pathVectorList.Add(vector);
		}
	}

}
