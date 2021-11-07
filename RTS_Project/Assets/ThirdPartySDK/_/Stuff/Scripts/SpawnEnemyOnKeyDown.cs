#region Info
// -----------------------------------------------------------------------
// SpawnEnemyOnKeyDown.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class SpawnEnemyOnKeyDown : MonoBehaviour
{

	[SerializeField] private Transform pfEnemyTransform;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			var spawnCount = 5;
			var spawnPosition = Vector3.zero;
			var radiusMin = 50f;
			var radiusMax = 150f;
			for (var i = 0; i < spawnCount; i++)
			{
				var enemyTransform = Instantiate(pfEnemyTransform,
						spawnPosition + UtilsClass.GetRandomDir() *
						Random.Range(radiusMin, radiusMax),
						Quaternion.identity);
			}
		}
	}
}
