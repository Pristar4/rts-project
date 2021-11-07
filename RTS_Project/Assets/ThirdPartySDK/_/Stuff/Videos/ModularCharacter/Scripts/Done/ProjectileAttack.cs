#region Info
// -----------------------------------------------------------------------
// ProjectileAttack.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class ProjectileAttack : MonoBehaviour, IAttack
{

	private Character_Base characterBase;
	private State state;

	private void Awake()
	{
		characterBase = GetComponent<Character_Base>();
		SetStateNormal();
	}

	public void Attack(Vector3 dir)
	{
		// Attack
		var boltShootDir
				= (UtilsClass.GetMouseWorldPosition() - transform.position)
				.normalized;
		var boltOffset = 10f;
		var boltSpawnPosition = transform.position + boltShootDir * boltOffset;
		ProjectileBolt.Create(boltSpawnPosition, boltShootDir);

		SetStateAttacking();

		var attackDir = boltShootDir;

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
					unitAnim => SetStateNormal(), null, null);
		}
		else
		{
			characterBase.GetUnitAnimation().PlayAnimForced(
					GameAssets.UnitAnimTypeEnum.dSwordTwoHandedBack_Sword,
					attackDir, 1f,
					unitAnim => SetStateNormal(), null, null);
		}
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
