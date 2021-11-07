#region Info
// -----------------------------------------------------------------------
// V_IObjectTransformBody.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion

namespace V_ObjectSystem
{

	public interface V_IObjectTransformBody
	{

		Vector3 ConvertBodyLocalPositionToWorldPosition(Vector3 position);
		void SetBodyMesh(Mesh mesh);
		void SetBodyMaterial(Material material);
		Transform GetBodyTransform();
		Material GetBodyMaterial();
	}

}
