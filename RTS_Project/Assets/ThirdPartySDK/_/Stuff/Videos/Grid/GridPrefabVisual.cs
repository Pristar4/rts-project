#region Info
// -----------------------------------------------------------------------
// GridPrefabVisual.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using UnityEngine;
#endregion
public class GridPrefabVisual : MonoBehaviour
{

	[SerializeField] private Transform pfGridPrefabVisualNode;
	private Grid<GridPrefabVisualObject> grid;
	private bool updateVisual;
	private Transform[,] visualNodeArray;

	private List<Transform> visualNodeList;

	public static GridPrefabVisual Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		visualNodeList = new List<Transform>();
	}

	private void Update()
	{
		if (updateVisual)
		{
			updateVisual = false;
			UpdateVisual(grid);
		}
	}

	public void Setup(Grid<GridPrefabVisualObject> grid)
	{
		this.grid = grid;
		visualNodeArray = new Transform[grid.GetWidth(), grid.GetHeight()];

		for (var x = 0; x < grid.GetWidth(); x++)
		for (var y = 0; y < grid.GetHeight(); y++)
		{
			var gridPosition = new Vector3(x, y) * grid.GetCellSize() +
			                   Vector3.one * grid.GetCellSize() * .5f;
			var visualNode = CreateVisualNode(gridPosition);
			visualNodeArray[x, y] = visualNode;
			visualNodeList.Add(visualNode);
		}

		HideNodeVisuals();

		grid.OnGridObjectChanged += Grid_OnGridObjectChanged;
	}

	private void Grid_OnGridObjectChanged(object sender,
	                                      Grid<GridPrefabVisualObject>.
			                                      OnGridObjectChangedEventArgs
			                                      e)
	{
		updateVisual = true;
	}

	public void UpdateVisual(Grid<GridPrefabVisualObject> grid)
	{
		HideNodeVisuals();

		for (var x = 0; x < grid.GetWidth(); x++)
		for (var y = 0; y < grid.GetHeight(); y++)
		{
			var gridObject = grid.GetGridObject(x, y);

			var visualNode = visualNodeArray[x, y];
			visualNode.gameObject.SetActive(true);
			SetupVisualNode(visualNode);
		}
	}

	private void HideNodeVisuals()
	{
		foreach (var visualNodeTransform in visualNodeList)
			visualNodeTransform.gameObject.SetActive(false);
	}

	private Transform CreateVisualNode(Vector3 position)
	{
		var visualNodeTransform = Instantiate(pfGridPrefabVisualNode, position,
				Quaternion.identity);
		return visualNodeTransform;
	}

	private void SetupVisualNode(Transform visualNodeTransform)
	{
	}

	/*
	 * Represents a single Grid Object
	 * */
	public class GridPrefabVisualObject
	{

		private readonly Grid<GridPrefabVisualObject> grid;
		private readonly int x;
		private readonly int y;
		private int value;

		public GridPrefabVisualObject(Grid<GridPrefabVisualObject> grid, int x,
		                              int y)
		{
			this.grid = grid;
			this.x = x;
			this.y = y;
		}

		public void SetValue(int value)
		{
			this.value = value;
			grid.TriggerGridObjectChanged(x, y);
		}

		public override string ToString()
		{
			return x + "," + y + "\n" + value;
		}
	}
}
