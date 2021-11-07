#region Info
// -----------------------------------------------------------------------
// ICharacterAnims.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using V_AnimationSystem;
#endregion
public interface ICharacterAnims
{

	void PlayIdleAnim();
	void PlayMoveAnim(Vector3 animDir);
	V_UnitAnimation GetUnitAnimation();
}
