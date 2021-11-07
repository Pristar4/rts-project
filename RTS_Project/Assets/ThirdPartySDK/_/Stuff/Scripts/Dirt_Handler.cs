#region Info
// -----------------------------------------------------------------------
// Dirt_Handler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using CodeMonkey.MonoBehaviours;
using UnityEngine;
#endregion
public class Dirt_Handler
{

	private static List<Dirt_Handler> instanceList = new List<Dirt_Handler>();
	private static readonly Vector3 baseSize = new Vector3(3f, 3f);

	private static float deltaTime, deltaTime5;

	private static float intervalTimer;
	private static readonly float intervalTimerMax = 0.1f;

	private static GameObject
			initGameObject; // Global game object used for initializing class, is destroyed on scene change
	private readonly int materialStarting;
	private readonly float materialTimerMax = 0.06f;
	private float eulerY;
	private int index;
	private int material;

	private float materialTimer;
	private Generic_Mesh_Script meshScript;
	private Vector3 pos = Vector3.zero;
	private float speed;

	private float timer = 1f;
	private Vector3 velocity = Vector3.zero;

	public Dirt_Handler()
	{
		materialStarting = Random.Range(0, 2) * 8;
		material = materialStarting; //Random.Range(0,8);
		materialTimer = materialTimerMax;
	}

	private static void InitIfNeeded()
	{
		if (initGameObject == null)
		{
			initGameObject = new GameObject("Dirt_Handler");
			ComponentActions.AddComponent(initGameObject, null, null, null,
					Update_Static);
		}
	}
	public static void ResetStatic()
	{
		instanceList = new List<Dirt_Handler>();
	}

	private void Update()
	{
		pos += velocity * (speed * deltaTime);
		speed -= speed * deltaTime5;
		//material = 8 - Mathf.FloorToInt(timer * 16f);
		//if (material < 0) material = 0;
		//if (material > 7) material = 7;
		materialTimer -= deltaTime;
		if (materialTimer < 0)
		{
			materialTimer = materialTimerMax;
			material++;
		}
		if (material > materialStarting + 7) material = materialStarting + 7;

		meshScript.updateGeneric(index, pos, eulerY, material, 0f, baseSize,
				false);

		timer -= deltaTime;
		if (timer < 0) instanceList.Remove(this);
	}
	public static void SpawnInterval(Vector3 loc, Vector3 dir)
	{
		if (intervalTimer <= 0f)
		{
			Spawn(1, loc, dir);
			intervalTimer = intervalTimerMax;
		}
	}
	public static void Spawn(Vector3 loc, Vector3 dir)
	{
		Spawn(3, loc, dir);
	}
	public static void Spawn(int amt, Vector3 loc, Vector3 dir)
	{
		InitIfNeeded();
		dir.Normalize();
		var baseDir = dir;

		loc.z = 0;
		for (var i = 0; i < amt; i++)
		{
			dir = baseDir;
			var handler = new Dirt_Handler();
			handler.pos = loc;
			handler.index = Generic_Mesh_Script.GetIndex("Mesh_Dirt");
			handler.meshScript = Generic_Mesh_Script.GetMeshScript("Mesh_Dirt");

			dir.x += Random.Range(-.4f, .4f);
			dir.y += Random.Range(-.4f, .4f);

			var velocity = dir.normalized;

			handler.speed = 5f * Random.Range(3f, 6f);
			handler.velocity = velocity;
			handler.eulerY = Random.Range(0, 360);
			Generic_Mesh_Script.AddGeneric("Mesh_Dirt", handler.pos,
					handler.eulerY,
					handler.material, 0f, baseSize, false);

			instanceList.Add(handler);
		}
	}
	public static void Update_Static()
	{
		deltaTime = Time.deltaTime;
		deltaTime5 = deltaTime * 5;
		intervalTimer -= deltaTime;
		for (var i = 0; i < instanceList.Count; i++) instanceList[i].Update();
	}
}
