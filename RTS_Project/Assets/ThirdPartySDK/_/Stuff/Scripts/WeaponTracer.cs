#region Info
// -----------------------------------------------------------------------
// WeaponTracer.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class WeaponTracer
{

	public static void Create(Vector3 fromPosition, Vector3 targetPosition)
	{
		var shootDir = (targetPosition - fromPosition).normalized;
		var distance = Vector3.Distance(fromPosition, targetPosition);
		var shootAngle = UtilsClass.GetAngleFromVectorFloat(shootDir);
		var spawnTracerPosition = fromPosition + shootDir * distance * .5f;
		var tracerMaterial = new Material(GameAssets.i.m_WeaponTracer);
		tracerMaterial.SetTextureScale("_MainTex",
				new Vector2(1f, distance / 256f));
		var worldMesh = new World_Mesh(null, spawnTracerPosition,
				new Vector3(1, 1),
				shootAngle - 90, 6f, distance, tracerMaterial, null, 10000);

		var frame = 0;
		var frameBase = 0;

		worldMesh.SetUVCoords(
				new World_Mesh.UVCoords(16 * frame + 64 * frameBase, 0, 16,
						256));
		var framerate = .016f;
		var timer = framerate;
		FunctionUpdater.Create(delegate
		{
			timer -= Time.deltaTime;
			if (timer < 0)
			{
				timer += framerate;
				frame++;
				if (frame >= 4)
				{
					worldMesh.DestroySelf();
					return true;
				}
				worldMesh.AddPosition(shootDir * 2f);
				worldMesh.SetUVCoords(
						new World_Mesh.UVCoords(16 * frame + 64 * frameBase, 0,
								16, 256));
			}
			return false;
		});
	}
}
