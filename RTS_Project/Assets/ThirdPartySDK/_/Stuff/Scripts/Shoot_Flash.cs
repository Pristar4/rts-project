#region Info
// -----------------------------------------------------------------------
// Shoot_Flash.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class Shoot_Flash
{

	private static List<Shoot_Flash> shootList = new List<Shoot_Flash>();
	private static float deltaTime;
	private static readonly Vector3 baseSize = new Vector3(20, 20);

	private static FunctionUpdater functionUpdater;
	private readonly int index;

	private float timer = .05f;

	private Shoot_Flash(Vector3 pos)
	{
		index = Generic_Mesh_Script.GetIndex("Mesh_Top");
		Generic_Mesh_Script.AddGeneric("Mesh_Top", pos, 0f, 0, 0f, baseSize,
				false);
	}

	public static void ResetStatic()
	{
		shootList = new List<Shoot_Flash>();
		if (functionUpdater != null) functionUpdater.DestroySelf();
		functionUpdater = null;
	}

	private static void Init()
	{
		if (functionUpdater == null) // Init
			functionUpdater = FunctionUpdater.Create(() => Update_Static());
	}

	private void Update()
	{
		timer -= deltaTime;
		if (timer < 0)
		{
			Generic_Mesh_Script.UpdateGeneric("Mesh_Top", index, Vector3.zero,
					0f, 0,
					0f, Vector3.zero, false);
			shootList.Remove(this);
		}
	}

	public static void AddFlash(Vector3 pos)
	{
		Init();
		var sh = new Shoot_Flash(pos);
		shootList.Add(sh);
	}
	private static void Update_Static()
	{
		deltaTime = Time.deltaTime;

		var tmpShootList = new List<Shoot_Flash>(shootList);
		for (var i = 0; i < tmpShootList.Count; i++) tmpShootList[i].Update();
	}
}
