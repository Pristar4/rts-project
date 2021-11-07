#region Info
// -----------------------------------------------------------------------
// IMoveVelocity.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public interface IMoveVelocity
{

	void SetVelocity(Vector3 velocityVector);
	void Disable();
	void Enable();
}
