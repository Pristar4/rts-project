#region Info
// -----------------------------------------------------------------------
// ItemWorldSpawner.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class ItemWorldSpawner : MonoBehaviour
{

	public Item item;

	private void Awake()
	{
		ItemWorld.SpawnItemWorld(transform.position, item);
		Destroy(gameObject);
	}
}
