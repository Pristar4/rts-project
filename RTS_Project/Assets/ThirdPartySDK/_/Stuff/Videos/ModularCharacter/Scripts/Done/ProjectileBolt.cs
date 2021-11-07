#region Info
// -----------------------------------------------------------------------
// ProjectileBolt.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class ProjectileBolt : MonoBehaviour
{
	private float destroyTimer;


	private Vector3 moveDir;

	private void Update()
	{
		var moveSpeed = 200f;
		transform.position += moveDir * moveSpeed * Time.deltaTime;

		destroyTimer -= Time.deltaTime;
		if (destroyTimer < 0f) Destroy(gameObject);
	}

	public static void Create(Vector3 spawnPosition, Vector3 moveDir)
	{
		var boltTransform = Instantiate(GameAssets.i.pfBolt, spawnPosition,
				Quaternion.identity);

		var projectileBolt = boltTransform.GetComponent<ProjectileBolt>();
		projectileBolt.Setup(moveDir);
	}

	private void Setup(Vector3 moveDir)
	{
		this.moveDir = moveDir;
		destroyTimer = 1f;
		transform.eulerAngles
				= new Vector3(0, 0,
						UtilsClass.GetAngleFromVectorFloat(moveDir));
	}
}
