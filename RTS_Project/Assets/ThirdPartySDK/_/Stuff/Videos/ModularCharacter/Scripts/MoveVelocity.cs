#region Info
// -----------------------------------------------------------------------
// MoveVelocity.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class MoveVelocity : MonoBehaviour, IMoveVelocity
{

	[SerializeField] private float moveSpeed;
	private Character_Base characterBase;
	private Rigidbody2D rigidbody2D;

	private Vector3 velocityVector;

	private void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();
		characterBase = GetComponent<Character_Base>();
	}

	private void FixedUpdate()
	{
		rigidbody2D.velocity = velocityVector * moveSpeed;

		characterBase.PlayMoveAnim(velocityVector);
	}

	public void SetVelocity(Vector3 velocityVector)
	{
		this.velocityVector = velocityVector;
	}

	public void Disable()
	{
		enabled = false;
		rigidbody2D.velocity = Vector3.zero;
	}

	public void Enable()
	{
		enabled = true;
	}
}
