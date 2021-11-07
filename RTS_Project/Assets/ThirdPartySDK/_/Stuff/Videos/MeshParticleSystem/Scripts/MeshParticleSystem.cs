#region Info
// -----------------------------------------------------------------------
// MeshParticleSystem.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using UnityEngine;
#endregion
public class MeshParticleSystem : MonoBehaviour
{

	private const int MAX_QUAD_AMOUNT = 15000;

	[SerializeField] private ParticleUVPixels[] particleUVPixelsArray;

	private Mesh mesh;
	private int quadIndex;
	private int[] triangles;
	private bool updateTriangles;
	private bool updateUV;

	private bool updateVertices;
	private Vector2[] uv;
	private UVCoords[] uvCoordsArray;
	private Vector3[] vertices;

	private void Awake()
	{
		mesh = new Mesh();

		vertices = new Vector3[4 * MAX_QUAD_AMOUNT];
		uv = new Vector2[4 * MAX_QUAD_AMOUNT];
		triangles = new int[6 * MAX_QUAD_AMOUNT];

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

		GetComponent<MeshFilter>().mesh = mesh;

		// Set up internal UV Normalized Array
		var material = GetComponent<MeshRenderer>().material;
		var mainTexture = material.mainTexture;
		var textureWidth = mainTexture.width;
		var textureHeight = mainTexture.height;

		var uvCoordsList = new List<UVCoords>();
		foreach (var particleUVPixels in particleUVPixelsArray)
		{
			var uvCoords = new UVCoords
			{
				uv00 = new Vector2(
						(float)particleUVPixels.uv00Pixels.x / textureWidth,
						(float)particleUVPixels.uv00Pixels.y / textureHeight),
				uv11 = new Vector2(
						(float)particleUVPixels.uv11Pixels.x / textureWidth,
						(float)particleUVPixels.uv11Pixels.y / textureHeight)
			};
			uvCoordsList.Add(uvCoords);
		}
		uvCoordsArray = uvCoordsList.ToArray();
	}

	private void LateUpdate()
	{
		if (updateVertices)
		{
			mesh.vertices = vertices;
			updateVertices = false;
		}
		if (updateUV)
		{
			mesh.uv = uv;
			updateUV = false;
		}
		if (updateTriangles)
		{
			mesh.triangles = triangles;
			updateTriangles = false;
		}
	}

	public int AddQuad(Vector3 position, float rotation, Vector3 quadSize,
	                   bool skewed, int uvIndex)
	{
		if (quadIndex >= MAX_QUAD_AMOUNT) return 0; // Mesh full

		UpdateQuad(quadIndex, position, rotation, quadSize, skewed, uvIndex);

		var spawnedQuadIndex = quadIndex;
		quadIndex++;

		return spawnedQuadIndex;
	}

	public void UpdateQuad(int quadIndex, Vector3 position, float rotation,
	                       Vector3 quadSize, bool skewed, int uvIndex)
	{
		//Relocate vertices
		var vIndex = quadIndex * 4;
		var vIndex0 = vIndex;
		var vIndex1 = vIndex + 1;
		var vIndex2 = vIndex + 2;
		var vIndex3 = vIndex + 3;

		if (skewed)
		{
			vertices[vIndex0] = position + Quaternion.Euler(0, 0, rotation) *
					new Vector3(-quadSize.x, -quadSize.y);
			vertices[vIndex1] = position + Quaternion.Euler(0, 0, rotation) *
					new Vector3(-quadSize.x, +quadSize.y);
			vertices[vIndex2] = position + Quaternion.Euler(0, 0, rotation) *
					new Vector3(+quadSize.x, +quadSize.y);
			vertices[vIndex3] = position + Quaternion.Euler(0, 0, rotation) *
					new Vector3(+quadSize.x, -quadSize.y);
		}
		else
		{
			vertices[vIndex0]
					= position + Quaternion.Euler(0, 0, rotation - 180) *
					quadSize;
			vertices[vIndex1]
					= position + Quaternion.Euler(0, 0, rotation - 270) *
					quadSize;
			vertices[vIndex2]
					= position + Quaternion.Euler(0, 0, rotation - 0) *
					quadSize;
			vertices[vIndex3]
					= position + Quaternion.Euler(0, 0, rotation - 90) *
					quadSize;
		}

		// UV
		var uvCoords = uvCoordsArray[uvIndex];
		uv[vIndex0] = uvCoords.uv00;
		uv[vIndex1] = new Vector2(uvCoords.uv00.x, uvCoords.uv11.y);
		uv[vIndex2] = uvCoords.uv11;
		uv[vIndex3] = new Vector2(uvCoords.uv11.x, uvCoords.uv00.y);

		//Create triangles
		var tIndex = quadIndex * 6;

		triangles[tIndex + 0] = vIndex0;
		triangles[tIndex + 1] = vIndex1;
		triangles[tIndex + 2] = vIndex2;

		triangles[tIndex + 3] = vIndex0;
		triangles[tIndex + 4] = vIndex2;
		triangles[tIndex + 5] = vIndex3;

		updateVertices = true;
		updateUV = true;
		updateTriangles = true;
	}

	public void DestroyQuad(int quadIndex)
	{
		// Destroy vertices
		var vIndex = quadIndex * 4;
		var vIndex0 = vIndex;
		var vIndex1 = vIndex + 1;
		var vIndex2 = vIndex + 2;
		var vIndex3 = vIndex + 3;

		vertices[vIndex0] = Vector3.zero;
		vertices[vIndex1] = Vector3.zero;
		vertices[vIndex2] = Vector3.zero;
		vertices[vIndex3] = Vector3.zero;

		updateVertices = true;
	}

	// Set in the Editor using Pixel Values
	[Serializable]
	public struct ParticleUVPixels
	{
		public Vector2Int uv00Pixels;
		public Vector2Int uv11Pixels;
	}

	// Holds normalized texture UV Coordinates
	private struct UVCoords
	{
		public Vector2 uv00;
		public Vector2 uv11;
	}
}
