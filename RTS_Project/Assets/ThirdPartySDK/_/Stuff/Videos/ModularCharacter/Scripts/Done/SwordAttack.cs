#region Info
// -----------------------------------------------------------------------
// SwordAttack.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class SwordAttack : MonoBehaviour, IAttack
{

	private Character_Base characterBase;
	private State state;

	private void Awake()
	{
		characterBase = GetComponent<Character_Base>();
		SetStateNormal();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			//Attack();
		}
	}

	public void Attack(Vector3 attackDir)
	{
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

	public void Attack(Vector3 attackDir, Action onAttackComplete)
	{
		// Attack
		SetStateAttacking();

		//Vector3 attackDir = (UtilsClass.GetMouseWorldPosition() - GetPosition()).normalized;

		//transform.position = transform.position + attackDir * 4f;

		var swordSlashTransform = Instantiate(GameAssets.i.pfSwordSlash,
				GetPosition() + attackDir * 13f,
				Quaternion.Euler(0, 0,
						UtilsClass.GetAngleFromVector(attackDir)));
		swordSlashTransform.GetComponent<SpriteAnimator>().onLoop
				= () => Destroy(swordSlashTransform.gameObject);

		var activeAnimType
				= characterBase.GetUnitAnimation().GetActiveAnimType();
		if (activeAnimType ==
		    GameAssets.UnitAnimTypeEnum.dSwordTwoHandedBack_Sword)
		{
			swordSlashTransform.localScale = new Vector3(
					swordSlashTransform.localScale.x,
					swordSlashTransform.localScale.y * -1,
					swordSlashTransform.localScale.z);
			characterBase.GetUnitAnimation().PlayAnimForced(
					GameAssets.UnitAnimTypeEnum.dSwordTwoHandedBack_Sword2,
					attackDir, 1f,
					unitAnim =>
					{
						SetStateNormal();
						onAttackComplete();
					}, null, null);
		}
		else
		{
			characterBase.GetUnitAnimation().PlayAnimForced(
					GameAssets.UnitAnimTypeEnum.dSwordTwoHandedBack_Sword,
					attackDir, 1f,
					unitAnim =>
					{
						SetStateNormal();
						onAttackComplete();
					}, null, null);
		}
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
