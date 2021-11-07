#region Info
// -----------------------------------------------------------------------
// PlayerSwapAimNormal.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion
public class PlayerSwapAimNormal : MonoBehaviour
{

	public enum WeaponType
	{
		Pistol,
		Shotgun,
		Sword,
		Punch
	}

	public event EventHandler<CharacterAim_Base.OnShootEventArgs> OnShoot;


	public void SetWeaponType(WeaponType weaponType)
	{
		this.weaponType = weaponType;

		switch (weaponType)
		{
			default:
			case WeaponType.Pistol:
				EnableAimSkeleton();
				characterAimBase.SetWeaponType(CharacterAim_Base.WeaponType
						.Pistol);
				break;
			case WeaponType.Shotgun:
				EnableAimSkeleton();
				characterAimBase.SetWeaponType(CharacterAim_Base.WeaponType
						.Shotgun);
				break;
			case WeaponType.Punch:
				EnableNormalSkeleton();
				playerPunch.enabled = true;
				playerSword.enabled = false;
				break;
			case WeaponType.Sword:
				EnableNormalSkeleton();
				playerPunch.enabled = false;
				playerSword.enabled = true;
				break;
		}
	}

	public void ConsumeHealthPotion()
	{
		FlashColor(Color.green);
	}

	public void ConsumeManaPotion()
	{
		FlashColor(Color.blue);
	}

	public void PlayWinAnimation()
	{
		playerBase.PlayWinAnimation();
	}

	public void PlayIdleAnim()
	{
		playerBase.PlayIdleAnim();
		characterAimBase.PlayIdleAnim();
	}

	public void PlayMoveAnim(Vector3 moveDir)
	{
		playerBase.PlayMoveAnim(moveDir);
		characterAimBase.PlayMoveAnim(moveDir);
	}

	#region Private
	private CharacterAim_Base characterAimBase;
	private PlayerAim playerAim;

	private Player_Base playerBase;
	private PlayerPunch playerPunch;
	private PlayerSword playerSword;

	private WeaponType weaponType;

	private void Awake()
	{
		characterAimBase = GetComponent<CharacterAim_Base>();
		playerAim = GetComponent<PlayerAim>();
		playerBase = GetComponent<Player_Base>();
		playerPunch = GetComponent<PlayerPunch>();
		playerSword = GetComponent<PlayerSword>();

		playerBase.enabled = false;
		playerPunch.enabled = false;
		playerSword.enabled = false;

		characterAimBase.OnShoot += CharacterAimBase_OnShoot;
	}

	private void Start()
	{
		SetWeaponType(WeaponType.Pistol);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T)) SetWeaponType(WeaponType.Punch);

		if (Input.GetKeyDown(KeyCode.Y)) SetWeaponType(WeaponType.Pistol);

		if (Input.GetKeyDown(KeyCode.U)) SetWeaponType(WeaponType.Sword);

		if (Input.GetKeyDown(KeyCode.I)) SetWeaponType(WeaponType.Shotgun);

		if (Input.GetKeyDown(KeyCode.O)) FlashColor(Color.green);
	}

	private void CharacterAimBase_OnShoot(object sender,
	                                      CharacterAim_Base.OnShootEventArgs e)
	{
		Shoot_Flash.AddFlash(e.gunEndPointPosition);
		WeaponTracer.Create(e.gunEndPointPosition, e.shootPosition);
		UtilsClass.ShakeCamera(.6f, .05f);
		SpawnBulletShellCasing(e.gunEndPointPosition, e.shootPosition);

		if (weaponType == WeaponType.Shotgun)
		{
			// Shotgun spread
			var shotgunShells = 4;
			for (var i = 0; i < shotgunShells; i++)
			{
				WeaponTracer.Create(e.gunEndPointPosition,
						e.shootPosition +
						UtilsClass.GetRandomDir() * Random.Range(-20f, 20f));
				if (i % 2 == 0)
					SpawnBulletShellCasing(e.gunEndPointPosition,
							e.shootPosition);
			}
		}

		// Any enemy hit?
		var raycastHit = Physics2D.Raycast(e.gunEndPointPosition,
				(e.shootPosition - e.gunEndPointPosition).normalized,
				Vector3.Distance(e.gunEndPointPosition, e.shootPosition));
		if (raycastHit.collider != null)
		{
			var enemyHandler
					= raycastHit.collider.gameObject
							.GetComponent<EnemyHandler>();
			if (enemyHandler != null) Debug.Log("Cannot Damage!");
			//enemyHandler.Damage(characterAimBase);
		}

		OnShoot?.Invoke(this, e);
	}

	private void SpawnBulletShellCasing(Vector3 gunEndPointPosition,
	                                    Vector3 shootPosition)
	{
		var shellSpawnPosition = gunEndPointPosition;
		var shootDir = (shootPosition - gunEndPointPosition).normalized;
		var backOffsetPosition = 8f;
		if (weaponType == WeaponType.Pistol) backOffsetPosition = 6f;
		shellSpawnPosition += shootDir * -1f * backOffsetPosition;

		var applyRotation = Random.Range(+130f, +95f);
		if (shootDir.x < 0) applyRotation *= -1f;

		var shellMoveDir
				= UtilsClass.ApplyRotationToVector(shootDir, applyRotation);

		//ShellParticleSystemHandler.Instance.SpawnShell(shellSpawnPosition, shellMoveDir);
	}

	private void EnableAimSkeleton()
	{
		playerBase.enabled = false;
		playerPunch.enabled = false;
		playerSword.enabled = false;

		characterAimBase.SetVObjectEnabled(true);
		characterAimBase.enabled = true;
		characterAimBase.RefreshBodySkeletonMesh();
		playerAim.enabled = true;
	}

	private void EnableNormalSkeleton()
	{
		characterAimBase.SetVObjectEnabled(false);
		characterAimBase.enabled = false;
		playerAim.enabled = false;

		playerBase.enabled = true;
		playerPunch.enabled = true;
		playerSword.enabled = true;
		playerBase.RefreshBodySkeletonMesh();
	}

	private void FlashColor(Color color)
	{
		GetComponent<MaterialTintColor>().SetTintColor(color);
	}
	#endregion
}
