#region Info
// -----------------------------------------------------------------------
// Test_Window_Graph.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using UnityEngine;
#endregion
public class Test_Window_Graph : MonoBehaviour
{

	[SerializeField] private Window_Graph graph;

	private void Start()
	{
		var valueList = new List<int>();

		for (var i = 0; i < Random.Range(50, 100); i++)
			valueList.Add(Random.Range(0, 20));

		graph.ShowGraph(valueList);
	}
}
