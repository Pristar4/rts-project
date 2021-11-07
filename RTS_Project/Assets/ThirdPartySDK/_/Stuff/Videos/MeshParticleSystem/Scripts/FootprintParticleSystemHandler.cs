#region Info
// -----------------------------------------------------------------------
// FootprintParticleSystemHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class FootprintParticleSystemHandler : MonoBehaviour
{

	private MeshParticleSystem meshParticleSystem;

	public static FootprintParticleSystemHandler Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		meshParticleSystem = GetComponent<MeshParticleSystem>();
	}

	public void SpawnFootprint(Vector3 position, Vector3 direction)
	{
		var quadSize = new Vector3(3f, 3f);
		var rotation = UtilsClass.GetAngleFromVectorFloat(direction) + 90f;
		meshParticleSystem.AddQuad(position, rotation, quadSize, false, 0);
	}
}
