#region Info
// -----------------------------------------------------------------------
// MeshParticleSystem_Testing.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class MeshParticleSystem_Testing : MonoBehaviour
{

	[SerializeField] private CharacterAimHandler characterAimHandler;

	private float nextSpawnDirtTime;
	private float nextSpawnFootprintTime;

	private void Start()
	{
		characterAimHandler.OnShoot += CharacterAimHandler_OnShoot;
	}

	private void Update()
	{
		TrySpawnDirtParticlesDelay();
		TrySpawnFootprintParticlesDelay();
	}

	private void TrySpawnDirtParticlesDelay()
	{
		if (Time.time >= nextSpawnDirtTime)
			if (characterAimHandler.IsMoving())
			{
				DirtParticleSystemHandler.Instance.SpawnDirt(
						characterAimHandler.GetPosition() + new Vector3(0, -3f),
						characterAimHandler.GetMoveDir() * -1f);
				nextSpawnDirtTime = Time.time + .08f;
			}
	}

	private void TrySpawnFootprintParticlesDelay()
	{
		if (Time.time >= nextSpawnFootprintTime)
			if (characterAimHandler.IsMoving())
			{
				FootprintParticleSystemHandler.Instance.SpawnFootprint(
						characterAimHandler.GetPosition() + new Vector3(0, -3f),
						characterAimHandler.GetMoveDir() * -1f);
				nextSpawnFootprintTime = Time.time + .3f;
			}
	}

	private void CharacterAimHandler_OnShoot(object sender,
	                                         CharacterAimHandler.
			                                         OnShootEventArgs
			                                         e)
	{
		var quadPosition = e.gunEndPointPosition;

		var shootDir = (e.shootPosition - e.gunEndPointPosition).normalized;
		quadPosition += shootDir * -1f * 8f;

		var applyRotation = Random.Range(+95f, +85f);
		if (shootDir.x < 0) applyRotation *= -1f;

		var shellMoveDir
				= UtilsClass.ApplyRotationToVector(shootDir, applyRotation);

		ShellParticleSystemHandler.Instance.SpawnShell(quadPosition,
				shellMoveDir);
		/*
		int uvIndex = Random.Range(0, 8);
		int spawnedQuadIndex = AddQuad(quadPosition, rotation, quadSize, true, uvIndex);
	
		FunctionUpdater.Create(() => {
		    quadPosition += new Vector3(1, 1) * 3f * Time.deltaTime;
		    //quadSize +=  new Vector3(1, 1) * Time.deltaTime;
		    //rotation += 360f * Time.deltaTime;
		    UpdateQuad(spawnedQuadIndex, quadPosition, rotation, quadSize, true, uvIndex);
		});
		*/
	}
}
