#region Info
// -----------------------------------------------------------------------
// KeyHolder.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using UnityEngine;
#endregion
public class KeyHolder : MonoBehaviour
{

	private List<Key.KeyType> keyList;

	private void Awake()
	{
		keyList = new List<Key.KeyType>();
	}


	private void OnTriggerEnter2D(Collider2D collider)
	{
		var key = collider.GetComponent<Key>();
		if (key != null)
		{
			AddKey(key.GetKeyType());
			Destroy(key.gameObject);
		}

		var keyDoor = collider.GetComponent<KeyDoor>();
		if (keyDoor != null)
		{
			if (ContainsKey(keyDoor.GetKeyType()))
			{
				// Currently holding Key to open this door
				RemoveKey(keyDoor.GetKeyType());
				keyDoor.OpenDoor();
			}
			else { keyDoor.PlayOpenFailAnim(); }
		}
	}

	public event EventHandler OnKeysChanged;

	public List<Key.KeyType> GetKeyList()
	{
		return keyList;
	}

	public void AddKey(Key.KeyType keyType)
	{
		Debug.Log("Added Key: " + keyType);
		keyList.Add(keyType);
		OnKeysChanged?.Invoke(this, EventArgs.Empty);
	}

	public void RemoveKey(Key.KeyType keyType)
	{
		keyList.Remove(keyType);
		OnKeysChanged?.Invoke(this, EventArgs.Empty);
	}

	public bool ContainsKey(Key.KeyType keyType)
	{
		return keyList.Contains(keyType);
	}
}
