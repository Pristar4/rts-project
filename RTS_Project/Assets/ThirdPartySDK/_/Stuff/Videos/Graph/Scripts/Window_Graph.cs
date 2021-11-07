#region Info
// -----------------------------------------------------------------------
// Window_Graph.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class Window_Graph : MonoBehaviour
{
	[SerializeField] private Sprite dotSprite;
	public IGraphVisual barChartVisual;
	private RectTransform dashContainer;
	private RectTransform dashTemplateX;
	private RectTransform dashTemplateY;
	private List<GameObject> gameObjectList;
	private Func<int, string> getAxisLabelX;
	private Func<float, string> getAxisLabelY;
	private Func<float, string> getTooltipLabel;
	private RectTransform graphContainer;
	private IGraphVisual graphVisual;
	private List<IGraphVisualObject> graphVisualObjectList;

	private bool isSetup;
	private RectTransform labelTemplateX;
	private RectTransform labelTemplateY;

	public IGraphVisual lineGraphVisual;
	private int maxVisibleValueAmount;
	private bool startYScaleAtZero;
	private GameObject tooltipGameObject;

	// Cached values
	private List<int> valueList;
	private float xSize;
	private List<RectTransform> yLabelList;

	private void Awake()
	{
		Setup();
	}

	private void Setup()
	{
		if (isSetup) return;
		isSetup = true;
		// Grab base objects references
		graphContainer
				= transform.Find("graphContainer")
						.GetComponent<RectTransform>();
		labelTemplateX = graphContainer.Find("labelTemplateX")
				.GetComponent<RectTransform>();
		labelTemplateY = graphContainer.Find("labelTemplateY")
				.GetComponent<RectTransform>();
		dashContainer = graphContainer.Find("dashContainer")
				.GetComponent<RectTransform>();
		dashTemplateX = dashContainer.Find("dashTemplateX")
				.GetComponent<RectTransform>();
		dashTemplateY = dashContainer.Find("dashTemplateY")
				.GetComponent<RectTransform>();
		tooltipGameObject = graphContainer.Find("tooltip").gameObject;

		labelTemplateX.gameObject.SetActive(false);
		labelTemplateY.gameObject.SetActive(false);
		dashTemplateX.gameObject.SetActive(false);
		dashTemplateY.gameObject.SetActive(false);

		startYScaleAtZero = true;
		gameObjectList = new List<GameObject>();
		yLabelList = new List<RectTransform>();
		graphVisualObjectList = new List<IGraphVisualObject>();

		lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite,
				Color.green, new Color(1, 1, 1, .5f), this);
		barChartVisual
				= new BarChartVisual(graphContainer, Color.white, .8f, this);

		valueList = new List<int> { 0 };

		HideTooltip();
	}

	private void ShowTooltip(string tooltipText, Vector2 anchoredPosition)
	{
		// Show Tooltip GameObject
		tooltipGameObject.SetActive(true);

		tooltipGameObject.GetComponent<RectTransform>().anchoredPosition
				= anchoredPosition;

		var tooltipUIText
				= tooltipGameObject.transform.Find("text").GetComponent<Text>();
		tooltipUIText.text = tooltipText;

		var textPaddingSize = 4f;
		var backgroundSize = new Vector2(
				tooltipUIText.preferredWidth + textPaddingSize * 2f,
				tooltipUIText.preferredHeight + textPaddingSize * 2f
		);

		tooltipGameObject.transform.Find("background")
				.GetComponent<RectTransform>()
				.sizeDelta = backgroundSize;

		// UI Visibility Sorting based on Hierarchy, SetAsLastSibling in order to show up on top
		tooltipGameObject.transform.SetAsLastSibling();
	}

	private void HideTooltip()
	{
		tooltipGameObject.SetActive(false);
	}

	public void SetGetAxisLabelX(Func<int, string> getAxisLabelX)
	{
		ShowGraph(valueList, graphVisual, maxVisibleValueAmount, getAxisLabelX,
				getAxisLabelY);
	}

	public void SetGetAxisLabelY(Func<float, string> getAxisLabelY)
	{
		ShowGraph(valueList, graphVisual, maxVisibleValueAmount, getAxisLabelX,
				getAxisLabelY);
	}

	public void SetGetTooltipLabel(Func<float, string> getTooltipLabel)
	{
		this.getTooltipLabel = getTooltipLabel;
	}

	public void IncreaseVisibleAmount()
	{
		ShowGraph(valueList, graphVisual, maxVisibleValueAmount + 1,
				getAxisLabelX,
				getAxisLabelY);
	}

	public void DecreaseVisibleAmount()
	{
		ShowGraph(valueList, graphVisual, maxVisibleValueAmount - 1,
				getAxisLabelX,
				getAxisLabelY);
	}

	private void SetGraphVisual(IGraphVisual graphVisual)
	{
		ShowGraph(valueList, graphVisual, maxVisibleValueAmount, getAxisLabelX,
				getAxisLabelY);
	}

	public void ShowGraph(List<int> valueList, IGraphVisual graphVisual = null,
	                      int maxVisibleValueAmount = -1,
	                      Func<int, string> getAxisLabelX = null,
	                      Func<float, string> getAxisLabelY = null)
	{
		Setup();
		if (valueList == null) valueList = new List<int> { 0 };
		//Debug.LogError("valueList is null!");
		//return;
		this.valueList = valueList;

		if (graphVisual == null) graphVisual = barChartVisual;
		this.graphVisual = graphVisual;

		if (maxVisibleValueAmount <= 0) // Show all if no amount specified
			maxVisibleValueAmount = valueList.Count;
		if (maxVisibleValueAmount >
		    valueList.Count) // Validate the amount to show the maximum
			maxVisibleValueAmount = valueList.Count;

		this.maxVisibleValueAmount = maxVisibleValueAmount;

		// Test for label defaults
		if (getAxisLabelX == null)
		{
			if (this.getAxisLabelX != null)
				getAxisLabelX = this.getAxisLabelX;
			else
				getAxisLabelX = delegate(int _i) { return _i.ToString(); };
		}
		if (getAxisLabelY == null)
		{
			if (this.getAxisLabelY != null)
				getAxisLabelY = this.getAxisLabelY;
			else
				getAxisLabelY = delegate(float _f)
				{
					return Mathf.RoundToInt(_f).ToString();
				};
		}

		this.getAxisLabelX = getAxisLabelX;
		this.getAxisLabelY = getAxisLabelY;

		// Clean up previous graph
		foreach (var gameObject in gameObjectList) Destroy(gameObject);
		gameObjectList.Clear();
		yLabelList.Clear();

		foreach (var graphVisualObject in graphVisualObjectList)
			graphVisualObject.CleanUp();
		graphVisualObjectList.Clear();

		graphVisual.CleanUp();

		// Grab the width and height from the container
		var graphWidth = graphContainer.sizeDelta.x;
		var graphHeight = graphContainer.sizeDelta.y;

		float yMinimum, yMaximum;
		CalculateYScale(out yMinimum, out yMaximum);

		// Set the distance between each point on the graph 
		xSize = graphWidth / (maxVisibleValueAmount + 1);

		// Cycle through all visible data points
		var xIndex = 0;
		for (var i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0);
				i < valueList.Count;
				i++)
		{
			var xPosition = xSize + xIndex * xSize;
			var yPosition = (valueList[i] - yMinimum) / (yMaximum - yMinimum) *
			                graphHeight;

			// Add data point visual
			string tooltipText;
			if (getTooltipLabel != null)
				tooltipText = getTooltipLabel(valueList[i]);
			else
				tooltipText = getAxisLabelY(valueList[i]);
			var graphVisualObject
					= graphVisual.CreateGraphVisualObject(
							new Vector2(xPosition, yPosition), xSize,
							tooltipText);
			graphVisualObjectList.Add(graphVisualObject);

			// Duplicate the x label template
			var labelX = Instantiate(labelTemplateX);
			labelX.SetParent(graphContainer, false);
			labelX.gameObject.SetActive(true);
			labelX.anchoredPosition = new Vector2(xPosition, -7f);
			labelX.GetComponent<Text>().text = getAxisLabelX(i);
			gameObjectList.Add(labelX.gameObject);

			// Duplicate the x dash template
			var dashX = Instantiate(dashTemplateX);
			dashX.SetParent(dashContainer, false);
			dashX.gameObject.SetActive(true);
			dashX.anchoredPosition = new Vector2(xPosition, -3f);
			dashX.sizeDelta = new Vector2(graphHeight, dashX.sizeDelta.y);
			gameObjectList.Add(dashX.gameObject);

			xIndex++;
		}

		// Set up separators on the y axis
		var separatorCount = 10;
		for (var i = 0; i <= separatorCount; i++)
		{
			// Duplicate the label template
			var labelY = Instantiate(labelTemplateY);
			labelY.SetParent(graphContainer, false);
			labelY.gameObject.SetActive(true);
			var normalizedValue = i * 1f / separatorCount;
			labelY.anchoredPosition
					= new Vector2(-7f, normalizedValue * graphHeight);
			labelY.GetComponent<Text>().text
					= getAxisLabelY(yMinimum +
					                normalizedValue * (yMaximum - yMinimum));
			yLabelList.Add(labelY);
			gameObjectList.Add(labelY.gameObject);

			// Duplicate the dash template
			var dashY = Instantiate(dashTemplateY);
			dashY.SetParent(dashContainer, false);
			dashY.gameObject.SetActive(true);
			dashY.anchoredPosition
					= new Vector2(-4f, normalizedValue * graphHeight);
			dashY.sizeDelta = new Vector2(graphWidth, dashY.sizeDelta.y);
			gameObjectList.Add(dashY.gameObject);
		}
	}

	public void UpdateLastIndexValue(int value)
	{
		UpdateValue(valueList.Count - 1, value);
	}

	public void UpdateValue(int index, int value)
	{
		float yMinimumBefore, yMaximumBefore;
		CalculateYScale(out yMinimumBefore, out yMaximumBefore);

		valueList[index] = value;

		//float graphWidth = graphContainer.sizeDelta.x;
		var graphHeight = graphContainer.sizeDelta.y;

		float yMinimum, yMaximum;
		CalculateYScale(out yMinimum, out yMaximum);

		var yScaleChanged
				= yMinimumBefore != yMinimum || yMaximumBefore != yMaximum;

		if (!yScaleChanged)
		{
			// Y Scale did not change, update only this value
			var xIndex
					= index - Mathf.Max(valueList.Count - maxVisibleValueAmount,
							0);
			var xPosition = xSize + xIndex * xSize;
			var yPosition = (value - yMinimum) / (yMaximum - yMinimum) *
			                graphHeight;

			// Add data point visual
			var tooltipText = getAxisLabelY(value);
			graphVisualObjectList[xIndex]
					.SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition),
							xSize,
							tooltipText);
		}
		else
		{
			// Y scale changed, update whole graph and y axis labels
			// Cycle through all visible data points
			UpdateAllVisiblePoints();
		}
	}

	public void UpdateAllVisiblePoints()
	{
		var graphWidth = graphContainer.sizeDelta.x;
		var graphHeight = graphContainer.sizeDelta.y;

		float yMinimum, yMaximum;
		CalculateYScale(out yMinimum, out yMaximum);

		// Cycle through all visible data points
		var xIndex = 0;
		for (var i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0);
				i < valueList.Count;
				i++)
		{
			var xPosition = xSize + xIndex * xSize;
			var yPosition = (valueList[i] - yMinimum) / (yMaximum - yMinimum) *
			                graphHeight;

			// Add data point visual
			var tooltipText = getAxisLabelY(valueList[i]);
			graphVisualObjectList[xIndex]
					.SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition),
							xSize,
							tooltipText);

			xIndex++;
		}

		for (var i = 0; i < yLabelList.Count; i++)
		{
			var normalizedValue = i * 1f / (yLabelList.Count - 1);
			yLabelList[i].GetComponent<Text>().text
					= getAxisLabelY(yMinimum +
					                normalizedValue * (yMaximum - yMinimum));
		}
	}

	private void CalculateYScale(out float yMinimum, out float yMaximum)
	{
		// Identify y Min and Max values
		yMaximum = valueList[0];
		yMinimum = valueList[0];

		for (var i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0);
				i < valueList.Count;
				i++)
		{
			var value = valueList[i];
			if (value > yMaximum) yMaximum = value;
			if (value < yMinimum) yMinimum = value;
		}

		var yDifference = yMaximum - yMinimum;
		if (yDifference <= 0) yDifference = 5f;
		yMaximum = yMaximum + yDifference * 0.2f;
		yMinimum = yMinimum - yDifference * 0.2f;

		if (startYScaleAtZero) yMinimum = 0f; // Start the graph at zero
	}



	/*
	 * Interface definition for showing visual for a data point
	 * */
	public interface IGraphVisual
	{

		IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition,
		                                           float graphPositionWidth,
		                                           string tooltipText);
		void CleanUp();
	}

	/*
	 * Represents a single Visual Object in the graph
	 * */
	public interface IGraphVisualObject
	{

		void SetGraphVisualObjectInfo(Vector2 graphPosition,
		                              float graphPositionWidth,
		                              string tooltipText);
		void CleanUp();
	}


	/*
	 * Displays data points as a Bar Chart
	 * */
	private class BarChartVisual : IGraphVisual
	{
		private readonly Color barColor;
		private readonly float barWidthMultiplier;

		private readonly RectTransform graphContainer;
		private readonly Window_Graph windowGraph;

		public BarChartVisual(RectTransform graphContainer, Color barColor,
		                      float barWidthMultiplier,
		                      Window_Graph windowGraph)
		{
			this.graphContainer = graphContainer;
			this.barColor = barColor;
			this.barWidthMultiplier = barWidthMultiplier;
			this.windowGraph = windowGraph;
		}

		public void CleanUp()
		{
		}

		public IGraphVisualObject CreateGraphVisualObject(
				Vector2 graphPosition, float graphPositionWidth,
				string tooltipText)
		{
			var barGameObject = CreateBar(graphPosition, graphPositionWidth);

			var barChartVisualObject = new BarChartVisualObject(barGameObject,
					barWidthMultiplier, windowGraph);
			barChartVisualObject.SetGraphVisualObjectInfo(graphPosition,
					graphPositionWidth, tooltipText);

			return barChartVisualObject;
		}

		private GameObject CreateBar(Vector2 graphPosition, float barWidth)
		{
			var gameObject = new GameObject("bar", typeof(Image));
			gameObject.transform.SetParent(graphContainer, false);
			gameObject.GetComponent<Image>().color = barColor;
			var rectTransform = gameObject.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
			rectTransform.sizeDelta
					= new Vector2(barWidth * barWidthMultiplier,
							graphPosition.y);
			rectTransform.anchorMin = new Vector2(0, 0);
			rectTransform.anchorMax = new Vector2(0, 0);
			rectTransform.pivot = new Vector2(.5f, 0f);

			// Add Button_UI Component which captures UI Mouse Events
			gameObject.AddComponent<Button_UI>();
			//Button_UI barButtonUI = gameObject.AddComponent<Button_UI>();

			return gameObject;
		}


		public class BarChartVisualObject : IGraphVisualObject
		{

			private readonly GameObject barGameObject;
			private readonly float barWidthMultiplier;
			private readonly Window_Graph windowGraph;

			public BarChartVisualObject(GameObject barGameObject,
			                            float barWidthMultiplier,
			                            Window_Graph windowGraph)
			{
				this.barGameObject = barGameObject;
				this.barWidthMultiplier = barWidthMultiplier;
				this.windowGraph = windowGraph;
			}

			public void SetGraphVisualObjectInfo(Vector2 graphPosition,
			                                     float graphPositionWidth,
			                                     string tooltipText)
			{
				var rectTransform = barGameObject.GetComponent<RectTransform>();
				rectTransform.anchoredPosition
						= new Vector2(graphPosition.x, 0f);
				rectTransform.sizeDelta
						= new Vector2(graphPositionWidth * barWidthMultiplier,
								graphPosition.y);

				var barButtonUI = barGameObject.GetComponent<Button_UI>();

				// Show Tooltip on Mouse Over
				barButtonUI.MouseOverOnceFunc = () =>
				{
					windowGraph.ShowTooltip(tooltipText, graphPosition);
				};

				// Hide Tooltip on Mouse Out
				barButtonUI.MouseOutOnceFunc = () =>
				{
					windowGraph.HideTooltip();
				};
			}

			public void CleanUp()
			{
				Destroy(barGameObject);
			}
		}
	}


	/*
	 * Displays data points as a Line Graph
	 * */
	private class LineGraphVisual : IGraphVisual
	{
		private readonly Color dotColor;
		private readonly Color dotConnectionColor;
		private readonly Sprite dotSprite;

		private readonly RectTransform graphContainer;
		private readonly Window_Graph windowGraph;
		private LineGraphVisualObject lastLineGraphVisualObject;

		public LineGraphVisual(RectTransform graphContainer, Sprite dotSprite,
		                       Color dotColor, Color dotConnectionColor,
		                       Window_Graph windowGraph)
		{
			this.graphContainer = graphContainer;
			this.dotSprite = dotSprite;
			this.dotColor = dotColor;
			this.dotConnectionColor = dotConnectionColor;
			this.windowGraph = windowGraph;
			lastLineGraphVisualObject = null;
		}

		public void CleanUp()
		{
			lastLineGraphVisualObject = null;
		}


		public IGraphVisualObject CreateGraphVisualObject(
				Vector2 graphPosition, float graphPositionWidth,
				string tooltipText)
		{
			var dotGameObject = CreateDot(graphPosition);


			GameObject dotConnectionGameObject = null;
			if (lastLineGraphVisualObject != null)
				dotConnectionGameObject = CreateDotConnection(
						lastLineGraphVisualObject.GetGraphPosition(),
						dotGameObject.GetComponent<RectTransform>()
								.anchoredPosition);

			var lineGraphVisualObject = new LineGraphVisualObject(dotGameObject,
					dotConnectionGameObject, lastLineGraphVisualObject,
					windowGraph);
			lineGraphVisualObject.SetGraphVisualObjectInfo(graphPosition,
					graphPositionWidth, tooltipText);

			lastLineGraphVisualObject = lineGraphVisualObject;

			return lineGraphVisualObject;
		}

		private GameObject CreateDot(Vector2 anchoredPosition)
		{
			var gameObject = new GameObject("dot", typeof(Image));
			gameObject.transform.SetParent(graphContainer, false);
			gameObject.GetComponent<Image>().sprite = dotSprite;
			gameObject.GetComponent<Image>().color = dotColor;
			var rectTransform = gameObject.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = anchoredPosition;
			rectTransform.sizeDelta = new Vector2(11, 11);
			rectTransform.anchorMin = new Vector2(0, 0);
			rectTransform.anchorMax = new Vector2(0, 0);

			// Add Button_UI Component which captures UI Mouse Events
			gameObject.AddComponent<Button_UI>();
			//Button_UI dotButtonUI = gameObject.AddComponent<Button_UI>();

			return gameObject;
		}

		private GameObject CreateDotConnection(Vector2 dotPositionA,
		                                       Vector2 dotPositionB)
		{
			var gameObject = new GameObject("dotConnection", typeof(Image));
			gameObject.transform.SetParent(graphContainer, false);
			gameObject.GetComponent<Image>().color = dotConnectionColor;
			gameObject.GetComponent<Image>().raycastTarget = false;
			var rectTransform = gameObject.GetComponent<RectTransform>();
			var dir = (dotPositionB - dotPositionA).normalized;
			var distance = Vector2.Distance(dotPositionA, dotPositionB);
			rectTransform.anchorMin = new Vector2(0, 0);
			rectTransform.anchorMax = new Vector2(0, 0);
			rectTransform.sizeDelta = new Vector2(distance, 3f);
			rectTransform.anchoredPosition
					= dotPositionA + dir * distance * .5f;
			rectTransform.localEulerAngles
					= new Vector3(0, 0,
							UtilsClass.GetAngleFromVectorFloat(dir));
			return gameObject;
		}


		public class LineGraphVisualObject : IGraphVisualObject
		{
			private readonly GameObject dotConnectionGameObject;

			private readonly GameObject dotGameObject;
			private readonly LineGraphVisualObject lastVisualObject;
			private readonly Window_Graph windowGraph;

			public LineGraphVisualObject(GameObject dotGameObject,
			                             GameObject dotConnectionGameObject,
			                             LineGraphVisualObject lastVisualObject,
			                             Window_Graph windowGraph)
			{
				this.dotGameObject = dotGameObject;
				this.dotConnectionGameObject = dotConnectionGameObject;
				this.lastVisualObject = lastVisualObject;
				this.windowGraph = windowGraph;

				if (lastVisualObject != null)
					lastVisualObject.OnChangedGraphVisualObjectInfo
							+= LastVisualObject_OnChangedGraphVisualObjectInfo;
			}

			public void SetGraphVisualObjectInfo(Vector2 graphPosition,
			                                     float graphPositionWidth,
			                                     string tooltipText)
			{
				var rectTransform = dotGameObject.GetComponent<RectTransform>();
				rectTransform.anchoredPosition = graphPosition;

				UpdateDotConnection();

				var dotButtonUI = dotGameObject.GetComponent<Button_UI>();

				// Show Tooltip on Mouse Over
				dotButtonUI.MouseOverOnceFunc = () =>
				{
					windowGraph.ShowTooltip(tooltipText, graphPosition);
				};

				// Hide Tooltip on Mouse Out
				dotButtonUI.MouseOutOnceFunc = () =>
				{
					windowGraph.HideTooltip();
				};

				if (OnChangedGraphVisualObjectInfo != null)
					OnChangedGraphVisualObjectInfo(this, EventArgs.Empty);
			}

			public void CleanUp()
			{
				Destroy(dotGameObject);
				Destroy(dotConnectionGameObject);
			}

			public event EventHandler OnChangedGraphVisualObjectInfo;

			private void LastVisualObject_OnChangedGraphVisualObjectInfo(
					object sender, EventArgs e)
			{
				UpdateDotConnection();
			}

			public Vector2 GetGraphPosition()
			{
				var rectTransform = dotGameObject.GetComponent<RectTransform>();
				return rectTransform.anchoredPosition;
			}

			private void UpdateDotConnection()
			{
				if (dotConnectionGameObject != null)
				{
					var dotConnectionRectTransform
							= dotConnectionGameObject
									.GetComponent<RectTransform>();
					var dir = (lastVisualObject.GetGraphPosition() -
					           GetGraphPosition())
							.normalized;
					var distance = Vector2.Distance(GetGraphPosition(),
							lastVisualObject.GetGraphPosition());
					dotConnectionRectTransform.sizeDelta
							= new Vector2(distance, 3f);
					dotConnectionRectTransform.anchoredPosition
							= GetGraphPosition() + dir * distance * .5f;
					dotConnectionRectTransform.localEulerAngles = new Vector3(0,
							0,
							UtilsClass.GetAngleFromVectorFloat(dir));
				}
			}
		}
	}
}
