#region Info
// -----------------------------------------------------------------------
// CharacterAimHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
using V_AnimationSystem;
using V_ObjectSystem;
#endregion
public class
		CharacterAimHandler : MonoBehaviour, EnemyHandler.IEnemyTargetable
{

	private bool isMoving;
	private Vector3 moveDir;

	// Use this for initialization
	private void Start()
	{
		var vObject = CreateBasicUnit(transform, new Vector3(500, 680), 30f,
				GameAssets.i.m_MarineSpriteSheet);
		var unitAnimation = vObject.GetLogic<V_UnitAnimation>();
		var unitSkeleton = vObject.GetLogic<V_UnitSkeleton>();
		var objectTransform = vObject.GetLogic<V_IObjectTransform>();

		var canShoot = true;

		var unitSkeletonCompositeWeapon
				= new V_UnitSkeleton_Composite_WeaponInvert(
						vObject, unitSkeleton,
						GameAssets.UnitAnimEnum.dMarine_AimWeaponRight,
						GameAssets.UnitAnimEnum.dMarine_AimWeaponRightInvertV,
						GameAssets.UnitAnimEnum.dMarine_ShootWeaponRight,
						GameAssets.UnitAnimEnum
								.dMarine_ShootWeaponRightInvertV);
		vObject.AddRelatedObject(unitSkeletonCompositeWeapon);
		unitSkeletonCompositeWeapon.SetActive();

		var unitSkeletonCompositeWalker_BodyHead
				= new V_UnitSkeleton_Composite_Walker(vObject, unitSkeleton,
						GameAssets.UnitAnimTypeEnum.dMarine_Walk,
						GameAssets.UnitAnimTypeEnum.dMarine_Idle,
						new[] { "Body", "Head" });
		vObject.AddRelatedObject(unitSkeletonCompositeWalker_BodyHead);

		var unitSkeletonCompositeWalker_Feet
				= new V_UnitSkeleton_Composite_Walker(
						vObject, unitSkeleton,
						GameAssets.UnitAnimTypeEnum.dMarine_Walk,
						GameAssets.UnitAnimTypeEnum.dMarine_Idle,
						new[] { "FootL", "FootR" });
		vObject.AddRelatedObject(unitSkeletonCompositeWalker_Feet);

		FunctionUpdater.Create(() =>
		{
			var targetPosition = UtilsClass.GetMouseWorldPosition();
			var aimDir = (targetPosition - vObject.GetPosition()).normalized;

			// Check for hits
			var gunEndPointPosition = vObject.GetLogic<V_UnitSkeleton>()
					.GetBodyPartPosition("MuzzleFlash");
			var raycastHit = Physics2D.Raycast(gunEndPointPosition,
					(targetPosition - gunEndPointPosition).normalized,
					Vector3.Distance(gunEndPointPosition, targetPosition));
			if (raycastHit.collider != null) // Hit something
				targetPosition = raycastHit.point;

			unitSkeletonCompositeWeapon.SetAimTarget(targetPosition);

			if (canShoot && Input.GetMouseButton(0))
			{
				// Shoot
				canShoot = false;
				// Replace Body and Head with Attack
				unitSkeleton.ReplaceBodyPartSkeletonAnim(
						GameAssets.UnitAnimTypeEnum.dMarine_Attack.GetUnitAnim(
								aimDir),
						"Body", "Head");
				// Shoot Composite Skeleton
				unitSkeletonCompositeWeapon.Shoot(targetPosition,
						() => { canShoot = true; });

				// Add Effects
				var shootFlashPosition = vObject.GetLogic<V_UnitSkeleton>()
						.GetBodyPartPosition("MuzzleFlash");
				if (OnShoot != null)
					OnShoot(this,
							new OnShootEventArgs
							{
								gunEndPointPosition = shootFlashPosition,
								shootPosition = targetPosition
							});

				//Shoot_Flash.AddFlash(shootFlashPosition);
				//WeaponTracer.Create(shootFlashPosition, targetPosition);
			}


			// Manual Movement
			isMoving = false;
			moveDir = new Vector3(0, 0);
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				moveDir.y = +1;
				isMoving = true;
			}
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			{
				moveDir.y = -1;
				isMoving = true;
			}
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				moveDir.x = -1;
				isMoving = true;
			}
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				moveDir.x = +1;
				isMoving = true;
			}
			moveDir.Normalize();

			var moveSpeed = 50f;
			var targetMoveToPosition = objectTransform.GetPosition() +
			                           moveDir * moveSpeed * Time.deltaTime;
			// Test if can move there
			raycastHit = Physics2D.Raycast(GetPosition() + moveDir * .1f,
					moveDir,
					Vector3.Distance(GetPosition(), targetMoveToPosition));
			if (raycastHit.collider != null)
			{
				// Hit something
			}
			else
			{
				// Can move, no wall
				objectTransform.SetPosition(targetMoveToPosition);
			}

			if (isMoving)
				Dirt_Handler.SpawnInterval(GetPosition(), moveDir * -1f);


			// Update Feet
			unitSkeletonCompositeWalker_Feet.UpdateBodyParts(isMoving, moveDir);

			if (canShoot) // Update Head and Body parts only when Not Shooting
				unitSkeletonCompositeWalker_BodyHead.UpdateBodyParts(isMoving,
						aimDir);
		});
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public void Damage(EnemyHandler enemyHandler)
	{
		Damage(enemyHandler.GetPosition());
	}
	public event EventHandler<OnShootEventArgs> OnShoot;

	public bool IsMoving()
	{
		return isMoving;
	}

	public Vector3 GetMoveDir()
	{
		return moveDir;
	}

	public void Damage(Vector3 attackerPosition)
	{
		var bloodDir = (GetPosition() - attackerPosition).normalized;
		Blood_Handler.SpawnBlood(GetPosition(), bloodDir);
		// Knockback
		transform.position += bloodDir * 3f;
	}


	private static V_Object CreateBasicUnit(Transform unitTransform,
	                                        Vector3 spawnPosition,
	                                        float walkerSpeed,
	                                        Material materialSpriteSheet)
	{
		var unitObject = V_Object.CreateObject(V_Main.UpdateType.Unit);

		var instantiatedTransform = unitTransform;

		V_IObjectTransform transform = new V_ObjectTransform_LateUpdate(
				instantiatedTransform,
				() => instantiatedTransform.transform.position,
				V_Main.RegisterOnLateUpdate, V_Main.DeregisterOnLateUpdate);
		unitObject.AddRelatedObject(transform);
		V_IObjectTransformBody transformBody
				= new V_ObjectTransformBody(instantiatedTransform.Find("Body"),
						materialSpriteSheet);
		unitObject.AddRelatedObject(transformBody);

		var unitSkeleton = new V_UnitSkeleton(1f,
				transformBody.ConvertBodyLocalPositionToWorldPosition,
				transformBody.SetBodyMesh);
		unitObject.AddRelatedObject(unitSkeleton);
		unitObject.AddActiveLogic(
				new V_ObjectLogic_SkeletonUpdater(unitSkeleton));
		var unitAnimation = new V_UnitAnimation(unitSkeleton);
		unitObject.AddRelatedObject(unitAnimation);

		unitObject.AddRelatedObject(
				new V_IHasWorldPosition_Class(transform.GetPosition));

		return unitObject;
	}

	public class OnShootEventArgs : EventArgs
	{
		public Vector3 gunEndPointPosition;
		public Vector3 shootPosition;
	}
}
