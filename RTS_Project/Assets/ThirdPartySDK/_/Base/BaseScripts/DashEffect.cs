﻿#region Info
// -----------------------------------------------------------------------
// DashEffect.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class DashEffect : MonoBehaviour
{

	public static void CreateDashEffect(Vector3 position, Vector3 dir,
	                                    float dashSize)
	{
		var dashTransform = Instantiate(GameAssets.i.pfDashEffect, position,
				Quaternion.identity);
		dashTransform.localEulerAngles
				= new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
		dashTransform.localScale = new Vector3(dashSize / 35f, 1, 1);
	}
}
