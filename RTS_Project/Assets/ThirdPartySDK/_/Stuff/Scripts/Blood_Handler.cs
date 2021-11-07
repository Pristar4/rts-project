#region Info
// -----------------------------------------------------------------------
// Blood_Handler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using CodeMonkey.MonoBehaviours;
using UnityEngine;
#endregion
public class Blood_Handler
{

	private static List<Blood_Handler> instanceList = new List<Blood_Handler>();
	//private static int layerMask = ~((1 << 10) | (1 << 2) | (1 << 12));
	private static readonly Vector3 baseSize = new Vector3(3f, 3f);

	private static float deltaTime, deltaTime5;

	private static GameObject
			initGameObject; // Global game object used for initializing class, is destroyed on scene change
	private readonly int material;
	private float eulerY;
	private int index;
	private Generic_Mesh_Script meshScript;
	private Vector3 pos = Vector3.zero;
	private float speed;

	private float timer = 1f;
	private Vector3 velocity = Vector3.zero;

	private Blood_Handler()
	{
		material = Random.Range(0, 8);
	}

	private static void InitIfNeeded()
	{
		if (initGameObject == null)
		{
			initGameObject = new GameObject("Blood_Handler");
			ComponentActions.AddComponent(initGameObject, null, null, null,
					Update_Static);
		}
	}
	public static void ResetStatic()
	{
		instanceList = new List<Blood_Handler>();
	}

	private void Update()
	{
		pos += velocity * (speed * deltaTime);
		speed -= speed * deltaTime5;

		meshScript.updateGeneric(index, pos, eulerY, material, 0f, baseSize,
				false);

		timer -= deltaTime;
		if (timer < 0) instanceList.Remove(this);
	}
	public static void SpawnBlood(Vector3 loc, Vector3 dir)
	{
		SpawnBlood(5, loc, dir);
	}
	public static void SpawnBlood(int amt, Vector3 loc, Vector3 dir)
	{
		InitIfNeeded();
		//Blood_Splatter.spawnSplatter(loc,dir);
		dir.Normalize();
		var baseDir = dir;

		loc.z = 0;
		for (var i = 0; i < amt; i++)
		{
			dir = baseDir;
			var handler = new Blood_Handler();
			handler.pos = loc;
			handler.index = Generic_Mesh_Script.GetIndex("Mesh_Blood");
			handler.meshScript
					= Generic_Mesh_Script.GetMeshScript("Mesh_Blood");

			dir.x += Random.Range(-.4f, .4f);
			dir.y += Random.Range(-.4f, .4f);

			var velocity = dir.normalized;

			//handler.speed = 13f * Random.Range(2f,7f);
			handler.speed = 20f * Random.Range(3f, 6f);
			handler.velocity = velocity;
			handler.eulerY = Random.Range(0, 360);
			Generic_Mesh_Script.AddGeneric("Mesh_Blood", handler.pos,
					handler.eulerY,
					handler.material, 0f, baseSize, false);

			instanceList.Add(handler);
		}
	}
	public static void Update_Static()
	{
		deltaTime = Time.deltaTime;
		deltaTime5 = deltaTime * 5;
		for (var i = 0; i < instanceList.Count; i++) instanceList[i].Update();
	}
}
