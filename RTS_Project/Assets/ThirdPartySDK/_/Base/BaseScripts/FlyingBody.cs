#region Info
// -----------------------------------------------------------------------
// FlyingBody.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class FlyingBody : MonoBehaviour
{
	private float eulerZ;

	private Vector3 flyDirection;
	private float spawnBloodTimer;
	private float timer;

	private void Update()
	{
		var flySpeed = 400f;
		transform.position += flyDirection * flySpeed * Time.deltaTime;

		var scaleSpeed = 7f;
		transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;

		var eulerSpeed = 360f * 4f;
		eulerZ += eulerSpeed * Time.deltaTime;
		transform.localEulerAngles = new Vector3(0, 0, eulerZ);

		spawnBloodTimer -= Time.deltaTime;
		if (spawnBloodTimer <= 0f)
		{
			var spawnBloodTimerMax = .016f;
			spawnBloodTimer = spawnBloodTimerMax;
			Blood_Handler.SpawnBlood(5, transform.position, flyDirection * -1f);
		}

		timer += Time.deltaTime;
		if (timer >= 1f) Destroy(gameObject);
	}

	public static void Create(Transform prefab, Vector3 spawnPosition,
	                          Vector3 flyDirection)
	{
		var flyingBodyTransform
				= Instantiate(prefab, spawnPosition, Quaternion.identity);
		var flyingBody
				= flyingBodyTransform.gameObject.AddComponent<FlyingBody>();
		flyingBody.Setup(flyDirection);
	}

	private void Setup(Vector3 flyDirection)
	{
		this.flyDirection = flyDirection;
		transform.localScale = Vector3.one * 2f;
		eulerZ = 0f;
	}
}
