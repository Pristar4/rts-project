#region Info
// -----------------------------------------------------------------------
// GameAssets.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Reflection;
using UnityEngine;
using V_AnimationSystem;
#endregion
public class GameAssets : MonoBehaviour
{

	private static GameAssets _i;




	public Sprite s_ShootFlash;

	public Transform pfSwordSlash;
	public Transform pfEnemy;
	public Transform pfEnemyFlyingBody;
	public Transform pfImpactEffect;
	public Transform pfDamagePopup;
	public Transform pfDashEffect;
	public Transform pfProjectileArrow;
	public Transform pfBolt;
	public Transform pfSmoke;

	public Material m_WeaponTracer;
	public Material m_MarineSpriteSheet;

	public Material m_DoorRed;
	public Material m_DoorGreen;
	public Material m_DoorBlue;

	public Material m_DoorKeyHoleRed;
	public Material m_DoorKeyHoleGreen;
	public Material m_DoorKeyHoleBlue;

	public static GameAssets i
	{
		get
		{
			if (_i == null)
				_i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
			return _i;
		}
	}






	public static class UnitAnimTypeEnum
	{

		public static UnitAnimType dSwordTwoHandedBack_Idle;
		public static UnitAnimType dSwordTwoHandedBack_Walk;
		public static UnitAnimType dSwordTwoHandedBack_Sword;
		public static UnitAnimType dSwordTwoHandedBack_Sword2;

		public static UnitAnimType dMinion_Idle;
		public static UnitAnimType dMinion_Walk;
		public static UnitAnimType dMinion_Attack;

		public static UnitAnimType dShielder_Idle;
		public static UnitAnimType dShielder_Walk;

		public static UnitAnimType dSwordShield_Idle;
		public static UnitAnimType dSwordShield_Walk;

		public static UnitAnimType dMarine_Idle;
		public static UnitAnimType dMarine_Walk;
		public static UnitAnimType dMarine_Attack;

		public static UnitAnimType dBareHands_Idle;
		public static UnitAnimType dBareHands_Walk;

		static UnitAnimTypeEnum()
		{
			V_Animation.Init();
			var fieldInfoArr
					= typeof(UnitAnimTypeEnum).GetFields(BindingFlags.Static |
							BindingFlags.Public);
			foreach (var fieldInfo in fieldInfoArr)
				if (fieldInfo != null)
					fieldInfo.SetValue(null,
							UnitAnimType.GetUnitAnimType(fieldInfo.Name));
		}
	}




	public static class UnitAnimEnum
	{

		public static UnitAnim dMarine_AimWeaponRight;
		public static UnitAnim dMarine_AimWeaponRightInvertV;
		public static UnitAnim dMarine_ShootWeaponRight;
		public static UnitAnim dMarine_ShootWeaponRightInvertV;

		static UnitAnimEnum()
		{
			V_Animation.Init();
			var fieldInfoArr
					= typeof(UnitAnimEnum).GetFields(BindingFlags.Static |
							BindingFlags.Public);
			foreach (var fieldInfo in fieldInfoArr)
				if (fieldInfo != null)
					fieldInfo.SetValue(null,
							UnitAnim.GetUnitAnim(fieldInfo.Name));
		}
	}
}
