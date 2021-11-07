#region Info
// -----------------------------------------------------------------------
// Bullet.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class Bullet : MonoBehaviour
{

	private Vector3 shootDir;

	private void Update()
	{
		var moveSpeed = 150f;
		transform.position += shootDir * moveSpeed * Time.deltaTime;

		// Distance based Hit Detection
		var hitDetectionSize = 3f;
		var target = Target.GetClosest(transform.position, hitDetectionSize);
		if (target != null)
		{
			target.Damage();
			Destroy(gameObject);
		}
	}

	public void Setup(Vector3 shootDir)
	{
		this.shootDir = shootDir;
		transform.eulerAngles
				= new Vector3(0, 0,
						UtilsClass.GetAngleFromVectorFloat(shootDir));
		Destroy(gameObject, 5f);
	}

	/*
	private void OnTriggerEnter2D(Collider2D collider) {
	    // Physics Hit Detection
	    Target target = collider.GetComponent<Target>();
	    if (target != null) {
	        // Hit a Target
	        target.Damage();
	        Destroy(gameObject);
	    }
	}
	*/
}
