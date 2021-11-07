#region Info
// -----------------------------------------------------------------------
// Player.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.MonoBehaviours;
using CodeMonkey.Utils;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion

namespace TopDownShooter
{
	public class Player : MonoBehaviour, Enemy.IEnemyTargetable
	{
		private Transform aimLightTransform;
		private bool canUseRifle;

		private bool canUseShotgun;
		private World_Bar healthBar;
		private HealthSystem healthSystem;
		private float invulnerableTime;
		private MaterialTintColor materialTintColor;

		private PlayerMain playerMain;

		private State state;

		private Weapon weapon;
		private Weapon weaponPistol;
		private Weapon weaponPunch;
		private Weapon weaponRifle;
		private Weapon weaponShotgun;

		public static Player Instance { get; private set; }

		private void Awake()
		{
			Instance = this;
			playerMain = GetComponent<PlayerMain>();
			materialTintColor = GetComponent<MaterialTintColor>();
			aimLightTransform = transform.Find("AimLight");
			state = State.Normal;
			healthSystem = new HealthSystem(100);
			healthBar = new World_Bar(transform, new Vector3(0, 10),
					new Vector3(12, 1.5f), Color.grey, Color.red, 1f, 10000,
					new World_Bar.Outline { color = Color.black, size = .5f });
			healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
			weaponPistol = new Weapon(Weapon.WeaponType.Pistol);
			weaponShotgun = new Weapon(Weapon.WeaponType.Shotgun);
			weaponRifle = new Weapon(Weapon.WeaponType.Rifle);
			weaponPunch = new Weapon(Weapon.WeaponType.Punch);
		}

		private void Start()
		{
			materialTintColor.SetMaterial(transform.Find("Body")
					.GetComponent<MeshRenderer>().material);
			SetWeapon(weaponRifle);
			playerMain.PlayerSwapAimNormal.OnShoot
					+= PlayerSwapAimNormal_OnShoot;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1)) SetWeapon(weaponPistol);

			if (Input.GetKeyDown(KeyCode.Alpha2) && canUseShotgun)
				SetWeapon(weaponShotgun);
			//SetWeapon(weaponPunch);

			if (Input.GetKeyDown(KeyCode.Alpha3) && canUseRifle)
				SetWeapon(weaponRifle);

			var mousePosition = UtilsClass.GetMouseWorldPosition();
			var aimDir = (mousePosition - GetPosition()).normalized;
			aimLightTransform.eulerAngles = new Vector3(0, 0,
					UtilsClass.GetAngleFromVectorFloat(aimDir));
		}

		private void OnTriggerEnter2D(Collider2D collider)
		{
			if (collider.GetComponent<PickupHealth>() != null)
			{
				// Health item!
				materialTintColor.SetTintColor(Color.green);
				healthSystem.Heal(healthSystem.GetHealthMax());
				Destroy(collider.gameObject);
			}

			if (collider.GetComponent<PickupShotgun>() != null)
			{
				// Shotgun
				materialTintColor.SetTintColor(Color.blue);
				SetCanUseShotgun();
				Destroy(collider.gameObject);
				OnPickedUpWeapon?.Invoke(Weapon.WeaponType.Shotgun,
						EventArgs.Empty);
			}

			if (collider.GetComponent<PickupRifle>() != null)
			{
				// Shotgun
				materialTintColor.SetTintColor(Color.blue);
				SetCanUseRifle();
				Destroy(collider.gameObject);
				OnPickedUpWeapon?.Invoke(Weapon.WeaponType.Rifle,
						EventArgs.Empty);
			}

			if (collider.GetComponent<Star>() != null)
			{
				// Star!
				// Game Win!
				collider.gameObject.SetActive(false);
				playerMain.PlayerSwapAimNormal.PlayWinAnimation();
				playerMain.PlayerMovementHandler.Disable();
				//transform.Find("Body").GetComponent<MeshRenderer>().material = GameAssets.i.m_PlayerWinOutline;
				healthBar.Hide();
				CameraFollow.Instance.SetCameraFollowPosition(GetPosition());
				CameraFollow.Instance.SetCameraZoom(35f);
				CinematicBars.Show_Static(150f, .6f);

				transform.Find("AimLight").gameObject.SetActive(false);
			}
		}

		public Vector3 GetPosition()
		{
			return transform.position;
		}

		public GameObject GetGameObject()
		{
			return gameObject;
		}

		public void Damage(Enemy attacker, float damageMultiplier)
		{
			var attackerPosition = GetPosition();
			if (attacker != null) attackerPosition = attacker.GetPosition();

			var damageAmount
					= Mathf.RoundToInt(8 * damageMultiplier *
					                   Random.Range(.8f, 1.2f));

			var isInvulnerable = Time.time < invulnerableTime;

			if (!isInvulnerable)
			{
				healthSystem.Damage(damageAmount);
				DamagePopup.Create(GetPosition(), damageAmount, true);
			}

			Sound_Manager.PlaySound(Sound_Manager.Sound.Player_Hit,
					GetPosition());

			var bloodDir = (GetPosition() - attackerPosition).normalized;
			BloodParticleSystemHandler.Instance.SpawnBlood(5, GetPosition(),
					bloodDir);
		}

		public event EventHandler OnWeaponChanged;
		public event EventHandler OnPickedUpWeapon;
		public event EventHandler OnDodged;

		public void SetCanUseShotgun()
		{
			canUseShotgun = true;
			SetWeapon(weaponShotgun);
		}

		public void SetCanUseRifle()
		{
			canUseRifle = true;
			SetWeapon(weaponRifle);
		}

		public void SetWeapon(Weapon weapon)
		{
			this.weapon = weapon;
			//playerMain.PlayerSwapAimNormal.SetWeapon(weapon);
			OnWeaponChanged?.Invoke(this, EventArgs.Empty);
		}

		public Weapon GetWeapon()
		{
			return weapon;
		}

		private void PlayerSwapAimNormal_OnShoot(object sender,
		                                         CharacterAim_Base.
				                                         OnShootEventArgs
				                                         e)
		{
			Shoot_Flash.AddFlash(e.gunEndPointPosition);
			WeaponTracer.Create(e.gunEndPointPosition, e.shootPosition);
			UtilsClass.ShakeCamera(.6f, .05f);
			SpawnBulletShellCasing(e.gunEndPointPosition, e.shootPosition);

			if (weapon.GetWeaponType() == Weapon.WeaponType.Shotgun)
			{
				// Shotgun spread
				var shotgunShells = 4;
				for (var i = 0; i < shotgunShells; i++)
				{
					WeaponTracer.Create(e.gunEndPointPosition,
							e.shootPosition +
							UtilsClass.GetRandomDir() *
							Random.Range(-20f, 20f));
					if (i % 2 == 0)
						SpawnBulletShellCasing(e.gunEndPointPosition,
								e.shootPosition);
				}
			}

			switch (weapon.GetWeaponType())
			{
				//case Weapon.WeaponType.Pistol: Sound_Manager.PlaySound(Sound_Manager.Sound.Pistol_Fire, GetPosition()); break;
				//case Weapon.WeaponType.Rifle: Sound_Manager.PlaySound(Sound_Manager.Sound.Rifle_Fire, GetPosition()); break;
				//case Weapon.WeaponType.Shotgun: Sound_Manager.PlaySound(Sound_Manager.Sound.Shotgun_Fire, GetPosition()); break;
			}

			// Any enemy hit?
			if (e.hitObject != null)
			{
				var enemy = e.hitObject.GetComponent<Enemy>();
				if (enemy != null)
					enemy.Damage(this, weapon.GetDamageMultiplier());
				var shieldFieldTransformer
						= e.hitObject.GetComponent<ShieldFieldTransformer>();
				if (shieldFieldTransformer != null)
					shieldFieldTransformer.Damage(this,
							weapon.GetDamageMultiplier());
			}
		}

		private void SpawnBulletShellCasing(Vector3 gunEndPointPosition,
		                                    Vector3 shootPosition)
		{
			var shellSpawnPosition = gunEndPointPosition;
			var shootDir = (shootPosition - gunEndPointPosition).normalized;
			var backOffsetPosition = 8f;
			if (weapon.GetWeaponType() == Weapon.WeaponType.Pistol)
				backOffsetPosition = 6f;
			shellSpawnPosition += shootDir * -1f * backOffsetPosition;

			var applyRotation = Random.Range(+130f, +95f);
			if (shootDir.x < 0) applyRotation *= -1f;
			//Sound_Manager.PlaySound(Sound_Manager.Sound.BulletShell, GetPosition());

			var shellMoveDir
					= UtilsClass.ApplyRotationToVector(shootDir, applyRotation);

			ShellParticleSystemHandler.Instance.SpawnShell(shellSpawnPosition,
					shellMoveDir);
		}

		private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
		{
			healthBar.SetSize(healthSystem.GetHealthNormalized());
		}

		public void Knockback(Vector3 knockbackDir, float knockbackAmount)
		{
			//transform.position += knockbackDir * knockbackAmount;
			playerMain.PlayerRigidbody2D.MovePosition(transform.position +
					knockbackDir * knockbackAmount);
		}

		public void Dodged()
		{
			Sound_Manager.PlaySound(Sound_Manager.Sound.Dash, GetPosition());
			invulnerableTime = Time.time + .2f;
			OnDodged?.Invoke(this, EventArgs.Empty);
		}

		private enum State
		{
			Normal
		}
	}
}
