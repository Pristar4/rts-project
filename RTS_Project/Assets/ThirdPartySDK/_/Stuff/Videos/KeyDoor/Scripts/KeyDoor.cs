#region Info
// -----------------------------------------------------------------------
// KeyDoor.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class KeyDoor : MonoBehaviour
{

	[SerializeField] private Key.KeyType keyType;

	private DoorAnims doorAnims;

	private void Awake()
	{
		doorAnims = GetComponent<DoorAnims>();
	}

	public Key.KeyType GetKeyType()
	{
		return keyType;
	}

	public void OpenDoor()
	{
		doorAnims.OpenDoor();
	}

	public void PlayOpenFailAnim()
	{
		doorAnims.PlayOpenFailAnim();
	}
}
