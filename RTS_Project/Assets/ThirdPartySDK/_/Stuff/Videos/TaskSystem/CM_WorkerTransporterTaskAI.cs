#region Info
// -----------------------------------------------------------------------
// CM_WorkerTransporterTaskAI.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey;
using UnityEngine;
#endregion

namespace CM_TaskSystem
{

	public class CM_WorkerTransporterTaskAI : MonoBehaviour
	{
		private State state;
		private CM_TaskSystem<CM_GameHandler.TransporterTask> taskSystem;
		private float waitingTimer;

		private CM_IWorker worker;

		private void Update()
		{
			switch (state)
			{
				case State.WaitingForNextTask:
					// Waiting to request a new task
					waitingTimer -= Time.deltaTime;
					if (waitingTimer <= 0)
					{
						var waitingTimerMax = .2f; // 200ms
						waitingTimer = waitingTimerMax;
						RequestNextTask();
					}
					break;
				case State.ExecutingTask:
					// Currently executing a task
					break;
			}
		}

		public void Setup(CM_IWorker worker,
		                  CM_TaskSystem<CM_GameHandler.TransporterTask>
				                  taskSystem)
		{
			this.worker = worker;
			this.taskSystem = taskSystem;
			state = State.WaitingForNextTask;
		}

		private void RequestNextTask()
		{
			CMDebug.TextPopup("RequestNextTask", worker.GetPosition());
			var task = taskSystem.RequestNextTask();
			if (task == null)
			{
				// No tasks available, wait before asking again
				state = State.WaitingForNextTask;
			}
			else
			{
				// There is a task available, execute it depending on type
				state = State.ExecutingTask;
				if (task is CM_GameHandler.TransporterTask
						.TakeWeaponFromSlotToPosition)
				{
					ExecuteTask_TakeWeaponFromSlotToPosition(
							task as CM_GameHandler.TransporterTask.
									TakeWeaponFromSlotToPosition);
					return;
				}
				// Task type unknown, error!
				Debug.LogError("Task type unknown!");
			}
		}

		private void ExecuteTask_TakeWeaponFromSlotToPosition(
				CM_GameHandler.TransporterTask.TakeWeaponFromSlotToPosition
						takeWeaponFromSlotToPositionTask)
		{
			worker.MoveTo(takeWeaponFromSlotToPositionTask.weaponSlotPosition,
					() =>
					{
						takeWeaponFromSlotToPositionTask.grabWeapon(this);
						worker.MoveTo(
								takeWeaponFromSlotToPositionTask.targetPosition,
								() =>
								{
									takeWeaponFromSlotToPositionTask
											.dropWeapon();
									state = State.WaitingForNextTask;
								});
					});
		}

		private enum State
		{
			WaitingForNextTask,
			ExecutingTask
		}
	}

}
