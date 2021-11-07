#region Info
// -----------------------------------------------------------------------
// CM_IWorker.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

namespace CM_TaskSystem
{

	public interface CM_IWorker
	{

		Vector3 GetPosition();
		void MoveTo(Vector3 position, Action onArrivedAtPosition = null);
		void PlayVictoryAnimation(Action onAnimationComplete);
		void PlayCleanUpAnimation(Action onAnimationComplete);
		GameObject GetGameObject();
	}

}
