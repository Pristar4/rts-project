#region Info
// -----------------------------------------------------------------------
// GridGradientVisual.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class GridGradientVisual : MonoBehaviour
{

	private Grid<GridGradientVisualObject> grid;
	private Mesh mesh;
	private bool updateMesh;

	private void Awake()
	{
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
	}

	private void LateUpdate()
	{
		if (updateMesh)
		{
			updateMesh = false;
			UpdateMeshVisual();
		}
	}

	public void SetGrid(Grid<GridGradientVisualObject> grid)
	{
		this.grid = grid;
		UpdateMeshVisual();

		grid.OnGridObjectChanged += Grid_OnGridValueChanged;
	}

	private void Grid_OnGridValueChanged(object sender,
	                                     Grid<GridGradientVisualObject>.
			                                     OnGridObjectChangedEventArgs e)
	{
		updateMesh = true;
	}

	private void UpdateMeshVisual()
	{
		MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(),
				out var vertices, out var uv, out var triangles);

		for (var x = 0; x < grid.GetWidth(); x++)
		for (var y = 0; y < grid.GetHeight(); y++)
		{
			var index = x * grid.GetHeight() + y;
			var quadSize = new Vector3(1, 1) * grid.GetCellSize();

			var gridObject = grid.GetGridObject(x, y);
			var gridValueNormalized = gridObject.GetValueNormalized();
			var gridValueUV = new Vector2(gridValueNormalized, 0f);

			MeshUtils.AddToMeshArrays(vertices, uv, triangles, index,
					grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize,
					gridValueUV, gridValueUV);
		}

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
	}

	/*
	 * Represents a single Grid Object
	 * */
	public class GridGradientVisualObject
	{

		private const int MIN = 0;
		private const int MAX = 100;

		private readonly Grid<GridGradientVisualObject> grid;
		private readonly int x;
		private readonly int y;
		private int value;

		public GridGradientVisualObject(Grid<GridGradientVisualObject> grid,
		                                int x,
		                                int y)
		{
			this.grid = grid;
			this.x = x;
			this.y = y;
		}

		public void AddValue(int addValue)
		{
			value += addValue;
			value = Mathf.Clamp(value, MIN, MAX);
			grid.TriggerGridObjectChanged(x, y);
		}

		public float GetValueNormalized()
		{
			return (float)value / MAX;
		}

		public override string ToString()
		{
			return value.ToString();
		}
	}
}
