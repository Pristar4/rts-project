#region Info
// -----------------------------------------------------------------------
// CM_GameHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;
#endregion

namespace CM_TaskSystem
{

	public class CM_GameHandler : MonoBehaviour
	{
		public static CM_TaskSystem<TransporterTask> transporterTaskSystem;

		[SerializeField] private Sprite floorShellsSprite;
		[SerializeField] private Sprite pistolSprite;
		[SerializeField] private Sprite whitePixelSprite;

		private CM_TaskSystem<Task> taskSystem;

		private List<WeaponSlot> weaponSlotList;

		private void Start()
		{
			taskSystem = new CM_TaskSystem<Task>();
			transporterTaskSystem = new CM_TaskSystem<TransporterTask>();

			CM_IWorker
					worker = null; // CM_Worker.Create(new Vector3(450, 500));
			var workerTaskAI
					= worker.GetGameObject().AddComponent<CM_WorkerTaskAI>();
			workerTaskAI.Setup(worker, taskSystem);

			worker = null; // CM_Worker.Create(new Vector3(550, 500));
			var workerTransporterTaskAI = worker.GetGameObject()
					.AddComponent<CM_WorkerTransporterTaskAI>();
			workerTransporterTaskAI.Setup(worker, transporterTaskSystem);

			weaponSlotList = new List<WeaponSlot>();
			var weaponSlotGameObject = SpawnWeaponSlot(new Vector3(500, 500));
			weaponSlotList.Add(new WeaponSlot(weaponSlotGameObject.transform));

			weaponSlotGameObject = SpawnWeaponSlot(new Vector3(500, 490));
			weaponSlotList.Add(new WeaponSlot(weaponSlotGameObject.transform));

			weaponSlotGameObject = SpawnWeaponSlot(new Vector3(500, 510));
			weaponSlotList.Add(new WeaponSlot(weaponSlotGameObject.transform));
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				// Spawn a pistol and queue the task to take it to a slot when possible
				var pistolGameObject
						= SpawnPistolSprite(UtilsClass.GetMouseWorldPosition());
				taskSystem.EnqueueTask(() =>
				{
					foreach (var weaponSlot in weaponSlotList)
						if (weaponSlot.IsEmpty())
						{
							// If the weapon slot is empty lets create the task to take it there
							weaponSlot.SetHasWeaponIncoming(true);
							Task task = new Task.TakeWeaponToWeaponSlot
							{
								weaponPosition
										= pistolGameObject.transform.position,
								weaponSlotPosition = weaponSlot.GetPosition(),
								grabWeapon = weaponWorkerTaskAI =>
								{
									// Grab weapon, parent the weapon to the worker
									pistolGameObject.transform.SetParent(
											weaponWorkerTaskAI
													.transform);
								},
								dropWeapon = () =>
								{
									// Drop weapon, set parent back to null
									pistolGameObject.transform.SetParent(null);
									// Notify the weapon slot that the weapon has arrived
									weaponSlot.SetWeaponTransform(
											pistolGameObject.transform);
								}
							};
							return task;
						}
					// Weapon slot not empty, keep looking
					// No weapon slot is empty, try again later
					return null;
				});
				//CMDebug.TextPopupMouse("Add Task: ShellFloorCleanUp, 5s delay");
				//SpawnFloorShellsWithTask(UtilsClass.GetMouseWorldPosition());
			}
			if (Input.GetMouseButtonDown(1))
			{
				CMDebug.TextPopupMouse("Add Task: MoveToPosition");
				//CM_TaskSystem.Task task = new CM_TaskSystem.Task.Victory { };
				//taskSystem.AddTask(task);
				Task task = new Task.MoveToPosition
						{ targetPosition = UtilsClass.GetMouseWorldPosition() };
				taskSystem.AddTask(task);
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				CMDebug.TextPopupMouse("Add Task: Victory");
				Task task = new Task.Victory();
				taskSystem.AddTask(task);
			}
		}


		private GameObject SpawnFloorShells(Vector3 position)
		{
			var gameObject
					= new GameObject("FloorShells", typeof(SpriteRenderer));
			gameObject.GetComponent<SpriteRenderer>().sprite
					= floorShellsSprite;
			gameObject.transform.position = position;
			return gameObject;
		}

		private void SpawnFloorShellsWithTask(Vector3 position)
		{
			var floorShellsGameObject = SpawnFloorShells(position);
			var floorShellsSpriteRenderer
					= floorShellsGameObject.GetComponent<SpriteRenderer>();

			var cleanUpTime = Time.time + 5f;
			taskSystem.EnqueueTask(() =>
			{
				if (Time.time >= cleanUpTime)
				{
					Task task = new Task.ShellFloorCleanUp
					{
						targetPosition
								= floorShellsGameObject.transform.position,
						cleanUpAction = () =>
						{
							// Clean Up Action, reduce alpha every frame until zero
							var alpha = 1f;
							FunctionUpdater.Create(() =>
							{
								alpha -= Time.deltaTime;
								floorShellsSpriteRenderer.color
										= new Color(1, 1, 1, alpha);
								if (alpha <= 0f)
									return true;
								return false;
							});
						}
					};
					return task;
				}
				return null;
			});
		}

		private GameObject SpawnPistolSprite(Vector3 position)
		{
			var gameObject
					= new GameObject("PistolSprite", typeof(SpriteRenderer));
			gameObject.GetComponent<SpriteRenderer>().sprite = pistolSprite;
			gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10000;
			gameObject.transform.position = position;
			gameObject.transform.localScale = new Vector3(7, 7);
			return gameObject;
		}

		private GameObject SpawnWeaponSlot(Vector3 position)
		{
			var gameObject
					= new GameObject("WeaponSlot", typeof(SpriteRenderer));
			gameObject.GetComponent<SpriteRenderer>().sprite = whitePixelSprite;
			gameObject.GetComponent<SpriteRenderer>().color
					= new Color(.5f, .5f, .5f);
			gameObject.transform.position = position;
			gameObject.transform.localScale = new Vector3(4, 4);
			return gameObject;
		}


		private class WeaponSlot
		{

			private readonly Transform weaponSlotTransform;
			private bool hasWeaponIncoming;
			private Transform weaponTransform;

			public WeaponSlot(Transform weaponSlotTransform)
			{
				this.weaponSlotTransform = weaponSlotTransform;
				SetWeaponTransform(null);
			}

			public bool IsEmpty()
			{
				return weaponTransform == null && !hasWeaponIncoming;
			}

			public void SetHasWeaponIncoming(bool hasWeaponIncoming)
			{
				this.hasWeaponIncoming = hasWeaponIncoming;
				UpdateSprite();
			}

			public void SetWeaponTransform(Transform weaponTransform)
			{
				this.weaponTransform = weaponTransform;
				hasWeaponIncoming = false;
				UpdateSprite();

				if (weaponTransform != null)
				{
					var task = new TransporterTask.TakeWeaponFromSlotToPosition
					{
						weaponSlotPosition = GetPosition(),
						targetPosition = new Vector3(600, 500),
						grabWeapon = weaponWorkerTaskAI =>
						{
							// Grab weapon, parent the weapon to the worker
							weaponTransform.SetParent(weaponWorkerTaskAI
									.transform);
							SetWeaponTransform(null);
						},
						dropWeapon = () =>
						{
							// Drop weapon, set parent back to null
							weaponTransform.SetParent(null);
						}
					};
					transporterTaskSystem.AddTask(task);
				}
				/*FunctionTimer.Create(() => {
				    if (weaponTransform != null) {
				        Destroy(weaponTransform.gameObject);
				        SetWeaponTransform(null);
				    }
				}, 4f);*/
			}

			public Vector3 GetPosition()
			{
				return weaponSlotTransform.position;
			}

			public void UpdateSprite()
			{
				weaponSlotTransform.GetComponent<SpriteRenderer>().color
						= IsEmpty() ? Color.grey : Color.red;
			}
		}

		public class Task : TaskBase
		{

			// Worker moves to Target Position
			public class MoveToPosition : Task
			{
				public Vector3 targetPosition;
			}

			// Workers plays Victory animation
			public class Victory : Task
			{
			}

			// Worker moves to target position, plays clean up animation, and executes clean up action
			public class ShellFloorCleanUp : Task
			{
				public Action cleanUpAction;
				public Vector3 targetPosition;
			}

			// Worker moves to weapon position, grabs the weapon, takes it to weapon slot, drops weapon
			public class TakeWeaponToWeaponSlot : Task
			{
				public Action dropWeapon;
				public Action<CM_WorkerTaskAI> grabWeapon;
				public Vector3 weaponPosition;
				public Vector3 weaponSlotPosition;
			}
		}

		public class TransporterTask : TaskBase
		{

			public class TakeWeaponFromSlotToPosition : TransporterTask
			{
				public Action dropWeapon;
				public Action<CM_WorkerTransporterTaskAI> grabWeapon;
				public Vector3 targetPosition;
				public Vector3 weaponSlotPosition;
			}
		}
	}

}
