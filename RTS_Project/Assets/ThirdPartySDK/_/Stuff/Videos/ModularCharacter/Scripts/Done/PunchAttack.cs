#region Info
// -----------------------------------------------------------------------
// PunchAttack.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class PunchAttack : MonoBehaviour, IAttack
{

	private Character_Base characterBase;
	private State state;

	private void Awake()
	{
		characterBase = GetComponent<Character_Base>();
		SetStateNormal();
	}

	public void Attack(Vector3 attackDir)
	{
		// Attack
		SetStateAttacking();

		//Vector3 attackDir = (UtilsClass.GetMouseWorldPosition() - GetPosition()).normalized;

		characterBase.PlayAttackAnimation(attackDir, SetStateNormal);
	}

	private void SetStateAttacking()
	{
		state = State.Attacking;
		GetComponent<IMoveVelocity>().Disable();
	}

	private void SetStateNormal()
	{
		state = State.Normal;
		GetComponent<IMoveVelocity>().Enable();
	}

	private Vector3 GetPosition()
	{
		return transform.position;
	}

	private enum State
	{
		Normal,
		Attacking
	}
}
