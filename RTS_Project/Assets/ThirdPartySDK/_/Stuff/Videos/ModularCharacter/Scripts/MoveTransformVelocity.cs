#region Info
// -----------------------------------------------------------------------
// MoveTransformVelocity.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class MoveTransformVelocity : MonoBehaviour, IMoveVelocity
{

	[SerializeField] private float moveSpeed;
	private Character_Base characterBase;

	private Vector3 velocityVector;

	private void Awake()
	{
		characterBase = GetComponent<Character_Base>();
	}

	private void Update()
	{
		transform.position += velocityVector * moveSpeed * Time.deltaTime;
		characterBase.PlayMoveAnim(velocityVector);
	}

	public void SetVelocity(Vector3 velocityVector)
	{
		this.velocityVector = velocityVector;
	}

	public void Disable()
	{
		enabled = false;
	}

	public void Enable()
	{
		enabled = true;
	}
}
