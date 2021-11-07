#region Info
// -----------------------------------------------------------------------
// V_UnitAnimation.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

namespace V_AnimationSystem
{

	/*
	 * Handles choosing the specific angle animation for a given UnitAnimType
	 * */
	public class V_UnitAnimation
	{
		private const float MathfRad2Deg = Mathf.Rad2Deg;

		// Cached for speed
		private static readonly Vector3 vector3Down = new Vector3(0, -1);
		private static readonly Vector3 vector3Zero = new Vector3(0, 0);
		private int activeAngle;

		private UnitAnim activeAnim;
		private UnitAnimType activeAnimType;


		private readonly V_UnitSkeleton unitSkeleton;

		public V_UnitAnimation(V_UnitSkeleton unitSkeleton)
		{
			this.unitSkeleton = unitSkeleton;
		}



		public UnitAnim GetActiveAnim()
		{
			return activeAnim;
		}
		public UnitAnimType GetActiveAnimType()
		{
			return activeAnimType;
		}


		public void ClearOnAnimInterruptedIfMatches(
				V_UnitSkeleton.OnAnimInterrupted OnAnimInterrupted)
		{
			unitSkeleton.ClearOnAnimInterruptedIfMatches(OnAnimInterrupted);
		}
		public void PlayAnimIdleIfOnCompleteMatches(
				Vector3 dir, V_UnitSkeleton.OnAnimComplete OnAnimComplete,
				UnitAnimType idleAnimType)
		{
			var angle = GetAngleFromVector(dir);
			var unitAnim = idleAnimType.GetUnitAnim(angle);
			if (unitSkeleton.PlayAnimIfOnCompleteMatches(unitAnim,
					OnAnimComplete))
			{
				// Playing anim
				activeAnimType = idleAnimType;
				activeAnim = unitAnim;
				activeAngle = angle;
			}
		}



		public void PlayAnim(UnitAnimType animType, float frameRateMod,
		                     V_UnitSkeleton.OnAnimComplete onAnimComplete,
		                     V_UnitSkeleton.OnAnimTrigger onAnimTrigger,
		                     V_UnitSkeleton.OnAnimInterrupted onAnimInterrupted)
		{
			PlayAnim(animType, activeAngle, frameRateMod, onAnimComplete,
					onAnimTrigger, onAnimInterrupted);
		}
		public void PlayAnim(UnitAnimType animType, Vector3 dir,
		                     float frameRateMod,
		                     V_UnitSkeleton.OnAnimComplete onAnimComplete,
		                     V_UnitSkeleton.OnAnimTrigger onAnimTrigger,
		                     V_UnitSkeleton.OnAnimInterrupted onAnimInterrupted)
		{
			var angle = GetAngleFromVector(dir);
			PlayAnim(animType, angle, frameRateMod, onAnimComplete,
					onAnimTrigger, onAnimInterrupted);
		}
		public void PlayAnim(UnitAnimType animType, int angle,
		                     float frameRateMod,
		                     V_UnitSkeleton.OnAnimComplete onAnimComplete,
		                     V_UnitSkeleton.OnAnimTrigger onAnimTrigger,
		                     V_UnitSkeleton.OnAnimInterrupted onAnimInterrupted)
		{
			// Ignores if same animType, same angle and same frameRateMod

			// 8 angles
			if (animType == activeAnimType && activeAngle == angle)
			{
				// Same anim, same angle
				return;
			}
			if (animType != activeAnimType)
			{
				// Different anim, same angle
			}
			PlayAnimForced(animType, angle, frameRateMod, onAnimComplete,
					onAnimTrigger, onAnimInterrupted);
		}
		public void PlayAnim(UnitAnim unitAnim)
		{
			PlayAnim(unitAnim, 1f, null, null, null);
		}
		public void PlayAnim(UnitAnim unitAnim, Action onAnimComplete)
		{
			PlayAnim(unitAnim, 1f, u => { onAnimComplete(); }, null, null);
		}
		public void PlayAnim(UnitAnim unitAnim, float frameRateMod,
		                     Action onAnimComplete)
		{
			PlayAnim(unitAnim, frameRateMod, u => { onAnimComplete(); }, null,
					null);
		}
		public void PlayAnim(UnitAnim unitAnim, float frameRateMod,
		                     V_UnitSkeleton.OnAnimComplete onAnimComplete,
		                     V_UnitSkeleton.OnAnimTrigger onAnimTrigger,
		                     V_UnitSkeleton.OnAnimInterrupted onAnimInterrupted)
		{
			// Ignores if same animType, same angle and same frameRateMod

			if (unitAnim == activeAnim) // Same anim, same angle
				return;
			PlayAnimForced(unitAnim, frameRateMod, onAnimComplete,
					onAnimTrigger, onAnimInterrupted);
		}


		public void PlayAnimForced(UnitAnimType animType, float frameRateMod,
		                           V_UnitSkeleton.OnAnimComplete onAnimComplete,
		                           V_UnitSkeleton.OnAnimTrigger onAnimTrigger,
		                           V_UnitSkeleton.OnAnimInterrupted
				                           onAnimInterrupted)
		{
			PlayAnimForced(animType, activeAngle, frameRateMod, onAnimComplete,
					onAnimTrigger, onAnimInterrupted);
		}
		public void PlayAnimForced(UnitAnimType animType, Vector3 dir,
		                           float frameRateMod,
		                           V_UnitSkeleton.OnAnimComplete onAnimComplete,
		                           V_UnitSkeleton.OnAnimTrigger onAnimTrigger,
		                           V_UnitSkeleton.OnAnimInterrupted
				                           onAnimInterrupted)
		{
			var angle = GetAngleFromVector(dir);
			PlayAnimForced(animType, angle, frameRateMod, onAnimComplete,
					onAnimTrigger, onAnimInterrupted);
		}
		public void PlayAnimForced(UnitAnimType animType, int angle,
		                           float frameRateMod,
		                           V_UnitSkeleton.OnAnimComplete onAnimComplete,
		                           V_UnitSkeleton.OnAnimTrigger onAnimTrigger,
		                           V_UnitSkeleton.OnAnimInterrupted
				                           onAnimInterrupted)
		{
			// Forcefully play animation no matter what is currently playing
			activeAnimType = animType;
			activeAngle = angle;

			var unitAnim = animType.GetUnitAnim(angle);
			activeAnim = unitAnim;

			unitSkeleton.PlayAnim(unitAnim, frameRateMod, onAnimComplete,
					onAnimTrigger, onAnimInterrupted);
		}
		public void PlayAnimForced(UnitAnim unitAnim, float frameRateMod,
		                           Action onAnimComplete)
		{
			PlayAnimForced(unitAnim, frameRateMod, u =>
			{
				if (onAnimComplete != null) onAnimComplete();
			}, null, null);
		}
		public void PlayAnimForced(UnitAnim unitAnim, float frameRateMod,
		                           V_UnitSkeleton.OnAnimComplete onAnimComplete,
		                           V_UnitSkeleton.OnAnimTrigger onAnimTrigger,
		                           V_UnitSkeleton.OnAnimInterrupted
				                           onAnimInterrupted)
		{
			// Forcefully play animation no matter what is currently playing
			activeAnimType = unitAnim.GetUnitAnimType();
			activeAnim = unitAnim;

			unitSkeleton.PlayAnim(unitAnim, frameRateMod, onAnimComplete,
					onAnimTrigger, onAnimInterrupted);
		}



		public void UpdateAnim(UnitAnimType animType, float frameRateMod,
		                       V_UnitSkeleton.OnAnimComplete onAnimComplete,
		                       V_UnitSkeleton.OnAnimTrigger onAnimTrigger,
		                       V_UnitSkeleton.OnAnimInterrupted
				                       onAnimInterrupted)
		{
			UpdateAnim(animType, activeAngle, frameRateMod, onAnimComplete,
					onAnimTrigger, onAnimInterrupted);
		}
		public void UpdateAnim(UnitAnimType animType, Vector3 dir,
		                       float frameRateMod,
		                       V_UnitSkeleton.OnAnimComplete onAnimComplete,
		                       V_UnitSkeleton.OnAnimTrigger onAnimTrigger,
		                       V_UnitSkeleton.OnAnimInterrupted
				                       onAnimInterrupted)
		{
			var angle = GetAngleFromVector(dir);
			UpdateAnim(animType, angle, frameRateMod, onAnimComplete,
					onAnimTrigger, onAnimInterrupted);
		}
		public void UpdateAnim(UnitAnimType animType, int angle,
		                       float frameRateMod,
		                       V_UnitSkeleton.OnAnimComplete onAnimComplete,
		                       V_UnitSkeleton.OnAnimTrigger onAnimTrigger,
		                       V_UnitSkeleton.OnAnimInterrupted
				                       onAnimInterrupted)
		{
			// Update animation if different angle
			if (animType == activeAnimType)
			{
				// Same anim, check angle
				if (activeAngle == angle) // Same angle, ignore
					return;
				// Different angle
				activeAngle = angle;
				var unitAnim = activeAnimType.GetUnitAnim(activeAngle);
				activeAnim = unitAnim;
				unitSkeleton.PlayAnimContinueFrames(unitAnim, frameRateMod,
						onAnimComplete, onAnimTrigger, onAnimInterrupted);
			}
			else
			{
				// Different anim
				PlayAnim(animType, angle, frameRateMod, onAnimComplete,
						onAnimTrigger, onAnimInterrupted);
			}
		}


		public void UpdateAnim(UnitAnimType animType, Vector3 dir,
		                       float frameRateMod)
		{
			var angle = GetAngleFromVector(dir);
			UpdateAnim(animType, angle, frameRateMod);
		}
		public void UpdateAnim(UnitAnimType animType, int angle,
		                       float frameRateMod)
		{
			// Update animation if different angle
			if (animType == activeAnimType)
			{
				// Same anim, check angle
				if (activeAngle == angle) // Same angle, ignore
					return;
				// Different angle
				activeAngle = angle;
				var unitAnim = activeAnimType.GetUnitAnim(activeAngle);
				activeAnim = unitAnim;
				unitSkeleton.PlayAnimContinueFrames(unitAnim, frameRateMod);
			}
			else
			{
				// Different anim
				PlayAnim(animType, angle, frameRateMod, null, null, null);
			}
		}














		public static int GetAngleFromVector(Vector3 dir)
		{
			if (dir.x == 0f && dir.y == 0f) dir = vector3Down;

			var n = Math.Atan2(dir.y, dir.x) * MathfRad2Deg;
			if (n < 0) n += 360;
			var angle = (int)Math.Round(n / 45);

			return angle;
		}

		public static int GetDeepAngleFromVector(Vector3 dir)
		{
			if (dir.x == 0f && dir.y == 0f) dir = vector3Down;

			var n = Math.Atan2(dir.y, dir.x) * MathfRad2Deg;
			if (n < 0) n += 360;
			var angle = (int)Math.Round(n / 22);
			//if (angle == 16) angle = 0;

			return angle;
		}
	}

}
