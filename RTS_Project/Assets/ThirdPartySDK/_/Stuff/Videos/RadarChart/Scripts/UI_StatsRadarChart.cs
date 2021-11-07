#region Info
// -----------------------------------------------------------------------
// UI_StatsRadarChart.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion
public class UI_StatsRadarChart : MonoBehaviour
{

	[SerializeField] private Material radarMaterial;
	[SerializeField] private Texture2D radarTexture2D;
	private CanvasRenderer radarMeshCanvasRenderer;

	private Stats stats;

	private void Awake()
	{
		radarMeshCanvasRenderer
				= transform.Find("radarMesh").GetComponent<CanvasRenderer>();
	}

	public void SetStats(Stats stats)
	{
		this.stats = stats;
		stats.OnStatsChanged += Stats_OnStatsChanged;
		UpdateStatsVisual();
	}

	private void Stats_OnStatsChanged(object sender, EventArgs e)
	{
		UpdateStatsVisual();
	}

	private void UpdateStatsVisual()
	{
		var mesh = new Mesh();

		var vertices = new Vector3[6];
		var uv = new Vector2[6];
		var triangles = new int[3 * 5];

		var angleIncrement = 360f / 5;
		var radarChartSize = 145f;

		var attackVertex = Quaternion.Euler(0, 0, -angleIncrement * 0) *
		                   Vector3.up * radarChartSize *
		                   stats.GetStatAmountNormalized(Stats.Type.Attack);
		var attackVertexIndex = 1;
		var defenceVertex = Quaternion.Euler(0, 0, -angleIncrement * 1) *
		                    Vector3.up * radarChartSize *
		                    stats.GetStatAmountNormalized(Stats.Type.Defence);
		var defenceVertexIndex = 2;
		var speedVertex = Quaternion.Euler(0, 0, -angleIncrement * 2) *
		                  Vector3.up *
		                  radarChartSize *
		                  stats.GetStatAmountNormalized(Stats.Type.Speed);
		var speedVertexIndex = 3;
		var manaVertex = Quaternion.Euler(0, 0, -angleIncrement * 3) *
		                 Vector3.up *
		                 radarChartSize *
		                 stats.GetStatAmountNormalized(Stats.Type.Mana);
		var manaVertexIndex = 4;
		var healthVertex = Quaternion.Euler(0, 0, -angleIncrement * 4) *
		                   Vector3.up * radarChartSize *
		                   stats.GetStatAmountNormalized(Stats.Type.Health);
		var healthVertexIndex = 5;

		vertices[0] = Vector3.zero;
		vertices[attackVertexIndex] = attackVertex;
		vertices[defenceVertexIndex] = defenceVertex;
		vertices[speedVertexIndex] = speedVertex;
		vertices[manaVertexIndex] = manaVertex;
		vertices[healthVertexIndex] = healthVertex;

		uv[0] = Vector2.zero;
		uv[attackVertexIndex] = Vector2.one;
		uv[defenceVertexIndex] = Vector2.one;
		uv[speedVertexIndex] = Vector2.one;
		uv[manaVertexIndex] = Vector2.one;
		uv[healthVertexIndex] = Vector2.one;

		triangles[0] = 0;
		triangles[1] = attackVertexIndex;
		triangles[2] = defenceVertexIndex;

		triangles[3] = 0;
		triangles[4] = defenceVertexIndex;
		triangles[5] = speedVertexIndex;

		triangles[6] = 0;
		triangles[7] = speedVertexIndex;
		triangles[8] = manaVertexIndex;

		triangles[9] = 0;
		triangles[10] = manaVertexIndex;
		triangles[11] = healthVertexIndex;

		triangles[12] = 0;
		triangles[13] = healthVertexIndex;
		triangles[14] = attackVertexIndex;


		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;

		radarMeshCanvasRenderer.SetMesh(mesh);
		radarMeshCanvasRenderer.SetMaterial(radarMaterial, radarTexture2D);
	}
}
