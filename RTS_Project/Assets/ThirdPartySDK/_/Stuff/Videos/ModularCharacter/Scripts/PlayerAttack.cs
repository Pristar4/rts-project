#region Info
// -----------------------------------------------------------------------
// PlayerAttack.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class PlayerAttack : MonoBehaviour
{

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
			GetComponent<IAttack>().Attack(new Vector3(1, 0, 0));
	}
}
