#region Info
// -----------------------------------------------------------------------
// V_ObjectTransformBody.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion

namespace V_ObjectSystem
{

	public class V_ObjectTransformBody : V_IObjectTransformBody
	{

		private readonly Transform bodyTransform;

		public V_ObjectTransformBody(Transform bodyTransform,
		                             Material material = null)
		{
			this.bodyTransform = bodyTransform;
			if (material != null) SetBodyMaterial(material);
		}

		public Vector3 ConvertBodyLocalPositionToWorldPosition(Vector3 position)
		{
			return GetBodyTransform().TransformPoint(position);
		}
		public void SetBodyMesh(Mesh mesh)
		{
			GetBodyTransform().GetComponent<MeshFilter>().mesh = mesh;
		}
		public void SetBodyMaterial(Material material)
		{
			GetBodyTransform().GetComponent<MeshRenderer>().material = material;
		}
		public Transform GetBodyTransform()
		{
			return bodyTransform;
		}
		public Material GetBodyMaterial()
		{
			return GetBodyTransform().GetComponent<MeshRenderer>().material;
		}
	}

}
