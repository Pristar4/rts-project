#region Info
// -----------------------------------------------------------------------
// AnimatedWalker.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using V_AnimationSystem;
#endregion
public class AnimatedWalker
{
	private readonly UnitAnimType idleAnimType;
	private readonly float idleFrameRate;
	private readonly V_UnitAnimation unitAnimation;
	private readonly UnitAnimType walkAnimType;
	private readonly float walkFrameRate;

	private Vector3 lastMoveVector;

	public AnimatedWalker(V_UnitAnimation unitAnimation,
	                      UnitAnimType idleAnimType, UnitAnimType walkAnimType,
	                      float idleFrameRate, float walkFrameRate)
	{
		this.unitAnimation = unitAnimation;
		this.idleAnimType = idleAnimType;
		this.walkAnimType = walkAnimType;
		this.idleFrameRate = idleFrameRate;
		this.walkFrameRate = walkFrameRate;
		lastMoveVector = new Vector3(0, -1);
		unitAnimation.PlayAnim(idleAnimType, lastMoveVector, idleFrameRate,
				null,
				null, null);
	}

	public void SetMoveVector(Vector3 moveVector)
	{
		if (moveVector == Vector3.zero)
		{
			// Idle
			unitAnimation.PlayAnim(idleAnimType, lastMoveVector, idleFrameRate,
					null,
					null, null);
		}
		else
		{
			// Moving
			lastMoveVector = moveVector;
			unitAnimation.PlayAnim(walkAnimType, lastMoveVector, walkFrameRate,
					null,
					null, null);
		}
	}

	public Vector3 GetLastMoveVector()
	{
		return lastMoveVector;
	}
}
