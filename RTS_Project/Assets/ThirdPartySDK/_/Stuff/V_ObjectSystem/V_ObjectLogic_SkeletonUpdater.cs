#region Info
// -----------------------------------------------------------------------
// V_ObjectLogic_SkeletonUpdater.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using V_AnimationSystem;
#endregion

namespace V_ObjectSystem
{

	public class V_ObjectLogic_SkeletonUpdater : V_IObjectActiveLogic,
			V_IDestroySelf
	{

		private readonly bool active;
		private readonly V_UnitSkeleton skeleton;
		private bool useUnscaledDeltaTime;

		public V_ObjectLogic_SkeletonUpdater(V_UnitSkeleton skeleton)
		{
			this.skeleton = skeleton;
			active = true;
			useUnscaledDeltaTime = false;
		}

		public void DestroySelf()
		{
			skeleton.DestroySelf();
		}

		public void Update(float deltaTime)
		{
			if (!active) return;
			if (useUnscaledDeltaTime)
				deltaTime /= V_TimeScaleManager.GetTimeScale();
			skeleton.Update(deltaTime);
		}
		public void UpdateAsSuperLogicActive(float deltaTime)
		{
			Update(deltaTime);
		}
		public void UpdateAsSuperLogicInactive(float deltaTime)
		{
			Update(deltaTime);
		}


		public void SetUseUnscaledDeltaTime(bool set)
		{
			useUnscaledDeltaTime = set;
		}
	}

}
