#region Info
// -----------------------------------------------------------------------
// BloodParticleSystemHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class BloodParticleSystemHandler : MonoBehaviour
{

	private MeshParticleSystem meshParticleSystem;
	private List<Single> singleList;

	public static BloodParticleSystemHandler Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		meshParticleSystem = GetComponent<MeshParticleSystem>();
		singleList = new List<Single>();
	}

	private void Update()
	{
		for (var i = 0; i < singleList.Count; i++)
		{
			var single = singleList[i];
			single.Update();
			if (single.IsParticleComplete())
			{
				singleList.RemoveAt(i);
				i--;
			}
		}
	}

	public void SpawnBlood(Vector3 position, Vector3 direction)
	{
		SpawnBlood(3, position, direction);
	}

	public void SpawnBlood(int bloodParticleCount, Vector3 position,
	                       Vector3 direction)
	{
		for (var i = 0; i < bloodParticleCount; i++)
			singleList.Add(new Single(position,
					UtilsClass.ApplyRotationToVector(direction,
							Random.Range(-15f, 15f)),
					meshParticleSystem));
	}


	/*
	 * Represents a single Dirt Particle
	 * */
	private class Single
	{
		private readonly Vector3 direction;

		private readonly MeshParticleSystem meshParticleSystem;
		private readonly int quadIndex;
		private readonly Vector3 quadSize;
		private readonly int uvIndex;
		private float moveSpeed;
		private Vector3 position;
		private float rotation;

		public Single(Vector3 position, Vector3 direction,
		              MeshParticleSystem meshParticleSystem)
		{
			this.position = position;
			this.direction = direction;
			this.meshParticleSystem = meshParticleSystem;

			quadSize = new Vector3(2.5f, 2.5f);
			rotation = Random.Range(0, 360f);
			moveSpeed = Random.Range(50f, 70f);
			uvIndex = Random.Range(0, 8);

			quadIndex
					= meshParticleSystem.AddQuad(position, rotation, quadSize,
							false,
							uvIndex);
		}

		public void Update()
		{
			position += direction * moveSpeed * Time.deltaTime;
			rotation += 360f * (moveSpeed / 10f) * Time.deltaTime;

			meshParticleSystem.UpdateQuad(quadIndex, position, rotation,
					quadSize,
					false, uvIndex);

			var slowDownFactor = 3.5f;
			moveSpeed -= moveSpeed * slowDownFactor * Time.deltaTime;
		}

		public bool IsParticleComplete()
		{
			return moveSpeed < .1f;
		}
	}
}
