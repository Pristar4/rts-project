#region Info
// -----------------------------------------------------------------------
// GridNodeSimple.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class GridNodeSimple
{

	private readonly Grid<GridNodeSimple> grid;
	private readonly int x;
	private readonly int y;

	public GridNodeSimple(Grid<GridNodeSimple> grid, int x, int y)
	{
		this.grid = grid;
		this.x = x;
		this.y = y;
	}

	public void DrawDebugQuadrant()
	{
		var worldPos00 = grid.GetWorldPosition(x, y);
		var worldPos10 = grid.GetWorldPosition(x + 1, y);
		var worldPos01 = grid.GetWorldPosition(x, y + 1);
		var worldPos11 = grid.GetWorldPosition(x + 1, y + 1);

		Debug.DrawLine(worldPos00, worldPos01, Color.white, 999f);
		Debug.DrawLine(worldPos00, worldPos10, Color.white, 999f);
		Debug.DrawLine(worldPos01, worldPos11, Color.white, 999f);
		Debug.DrawLine(worldPos10, worldPos11, Color.white, 999f);
	}
}
