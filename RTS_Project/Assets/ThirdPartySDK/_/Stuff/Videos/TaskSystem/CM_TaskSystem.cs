#region Info
// -----------------------------------------------------------------------
// CM_TaskSystem.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
#endregion

namespace CM_TaskSystem
{

	public class QueuedTask<TTask> where TTask : TaskBase
	{

		private readonly Func<TTask> tryGetTaskFunc;

		public QueuedTask(Func<TTask> tryGetTaskFunc)
		{
			this.tryGetTaskFunc = tryGetTaskFunc;
		}

		public TTask TryDequeueTask()
		{
			return tryGetTaskFunc();
		}
	}

	// Base Task class
	public abstract class TaskBase
	{
	}

	public class CM_TaskSystem<TTask> where TTask : TaskBase
	{
		private readonly List<QueuedTask<TTask>>
				queuedTaskList; // Any queued task must be validated before being dequeued

		private readonly List<TTask>
				taskList; // List of tasks ready to be executed

		public CM_TaskSystem()
		{
			taskList = new List<TTask>();
			queuedTaskList = new List<QueuedTask<TTask>>();
			FunctionPeriodic.Create(DequeueTasks, .2f);
		}

		public TTask RequestNextTask()
		{
			// Worker requesting a task
			if (taskList.Count > 0)
			{
				// Give worker the first task and remove it from the list
				var task = taskList[0];
				taskList.RemoveAt(0);
				return task;
			}
			// No tasks are available
			return null;
		}

		public void AddTask(TTask task)
		{
			taskList.Add(task);
		}

		public void EnqueueTask(QueuedTask<TTask> queuedTask)
		{
			queuedTaskList.Add(queuedTask);
		}

		public void EnqueueTask(Func<TTask> tryGetTaskFunc)
		{
			var queuedTask = new QueuedTask<TTask>(tryGetTaskFunc);
			queuedTaskList.Add(queuedTask);
		}

		private void DequeueTasks()
		{
			for (var i = 0; i < queuedTaskList.Count; i++)
			{
				var queuedTask = queuedTaskList[i];
				var task = queuedTask.TryDequeueTask();
				if (task != null)
				{
					// Task dequeued! Add to normal list
					AddTask(task);
					queuedTaskList.RemoveAt(i);
					i--;
					CMDebug.TextPopupMouse("Task Dequeued");
				}
			}
		}
	}

}
