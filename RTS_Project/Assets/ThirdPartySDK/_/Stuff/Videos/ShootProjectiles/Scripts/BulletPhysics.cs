#region Info
// -----------------------------------------------------------------------
// BulletPhysics.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class BulletPhysics : MonoBehaviour
{

	private void OnTriggerEnter2D(Collider2D collider)
	{
		// Physics Hit Detection
		var target = collider.GetComponent<Target>();
		if (target != null)
		{
			// Hit a Target
			target.Damage();
			Destroy(gameObject);
		}
	}

	public void Setup(Vector3 shootDir)
	{
		var rigidbody2D = GetComponent<Rigidbody2D>();
		var moveSpeed = 150f;
		rigidbody2D.AddForce(shootDir * moveSpeed, ForceMode2D.Impulse);

		transform.eulerAngles
				= new Vector3(0, 0,
						UtilsClass.GetAngleFromVectorFloat(shootDir));
		Destroy(gameObject, 5f);
	}
}
