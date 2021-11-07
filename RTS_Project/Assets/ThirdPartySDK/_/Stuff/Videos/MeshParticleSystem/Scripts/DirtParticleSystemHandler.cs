#region Info
// -----------------------------------------------------------------------
// DirtParticleSystemHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using UnityEngine;
#endregion
public class DirtParticleSystemHandler : MonoBehaviour
{

	private MeshParticleSystem meshParticleSystem;
	private List<Single> singleList;

	public static DirtParticleSystemHandler Instance { get; private set; }

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
				single.DestroySelf();
				singleList.RemoveAt(i);
				i--;
			}
		}
	}

	public void SpawnDirt(Vector3 position, Vector3 direction)
	{
		singleList.Add(new Single(position, direction, meshParticleSystem));
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
		private readonly float uvIndexTimerMax;
		private float moveSpeed;
		private Vector3 position;
		private int uvIndex;
		private float uvIndexTimer;

		public Single(Vector3 position, Vector3 direction,
		              MeshParticleSystem meshParticleSystem)
		{
			this.position = position;
			this.direction = direction;
			this.meshParticleSystem = meshParticleSystem;

			quadSize = new Vector3(2.5f, 2.5f);
			moveSpeed = Random.Range(20f, 30f);
			uvIndex = 0;
			uvIndexTimerMax = 1f / 10;

			quadIndex
					= meshParticleSystem.AddQuad(position, 0f, quadSize, false,
							uvIndex);
		}

		public void Update()
		{
			uvIndexTimer += Time.deltaTime;
			if (uvIndexTimer >= uvIndexTimerMax)
			{
				uvIndexTimer -= uvIndexTimerMax;
				uvIndex++;
			}
			position += direction * moveSpeed * Time.deltaTime;

			meshParticleSystem.UpdateQuad(quadIndex, position, 0f, quadSize,
					false,
					uvIndex);

			var slowDownFactor = 3.5f;
			moveSpeed -= moveSpeed * slowDownFactor * Time.deltaTime;
		}

		public bool IsParticleComplete()
		{
			return uvIndex >= 8 || moveSpeed < .1f;
		}

		public void DestroySelf()
		{
			meshParticleSystem.DestroyQuad(quadIndex);
		}
	}
}
