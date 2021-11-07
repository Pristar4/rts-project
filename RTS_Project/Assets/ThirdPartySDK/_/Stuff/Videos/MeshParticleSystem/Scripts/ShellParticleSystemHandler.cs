#region Info
// -----------------------------------------------------------------------
// ShellParticleSystemHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using UnityEngine;
#endregion
public class ShellParticleSystemHandler : MonoBehaviour
{

	private MeshParticleSystem meshParticleSystem;
	private List<Single> singleList;

	public static ShellParticleSystemHandler Instance { get; private set; }

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
			if (single.IsMovementComplete())
			{
				singleList.RemoveAt(i);
				i--;
			}
		}
	}

	public void SpawnShell(Vector3 position, Vector3 direction)
	{
		singleList.Add(new Single(position, direction, meshParticleSystem));
	}


	/*
	 * Represents a single Shell
	 * */
	private class Single
	{
		private readonly Vector3 direction;

		private readonly MeshParticleSystem meshParticleSystem;
		private readonly int quadIndex;
		private readonly Vector3 quadSize;
		private float moveSpeed;
		private Vector3 position;
		private float rotation;

		public Single(Vector3 position, Vector3 direction,
		              MeshParticleSystem meshParticleSystem)
		{
			this.position = position;
			this.direction = direction;
			this.meshParticleSystem = meshParticleSystem;

			quadSize = new Vector3(.5f, 1f);
			rotation = Random.Range(0, 360f);
			moveSpeed = Random.Range(30f, 50f);

			quadIndex
					= meshParticleSystem.AddQuad(position, rotation, quadSize,
							true, 0);
		}

		public void Update()
		{
			position += direction * moveSpeed * Time.deltaTime;
			rotation += 360f * (moveSpeed / 10f) * Time.deltaTime;

			meshParticleSystem.UpdateQuad(quadIndex, position, rotation,
					quadSize,
					true, 0);

			var slowDownFactor = 3.5f;
			moveSpeed -= moveSpeed * slowDownFactor * Time.deltaTime;
		}

		public bool IsMovementComplete()
		{
			return moveSpeed < .1f;
		}
	}
}
