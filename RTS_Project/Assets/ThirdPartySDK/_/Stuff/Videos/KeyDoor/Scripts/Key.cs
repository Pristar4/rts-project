#region Info
// -----------------------------------------------------------------------
// Key.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class Key : MonoBehaviour
{

	public enum KeyType
	{
		Red,
		Green,
		Blue
	}

	[SerializeField] private KeyType keyType;

	public KeyType GetKeyType()
	{
		return keyType;
	}

	public static Color GetColor(KeyType keyType)
	{
		switch (keyType)
		{
			default:
			case KeyType.Red: return Color.red;
			case KeyType.Green: return Color.green;
			case KeyType.Blue:  return Color.blue;
		}
	}
}
