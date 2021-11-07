#region Info
// -----------------------------------------------------------------------
// PathQueue.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
namespace GridPathfindingSystem
{

	public class PathQueue
	{
		public GridPathfinding.OnPathCallback callback;
		public int endX;
		public int endY;

		public int startX;
		public int startY;

		public PathQueue(int _startX, int _startY, int _endX, int _endY,
		                 GridPathfinding.OnPathCallback _callback)
		{
			startX = _startX;
			startY = _startY;
			endX = _endX;
			endY = _endY;
			callback = _callback;
		}
	}

}
