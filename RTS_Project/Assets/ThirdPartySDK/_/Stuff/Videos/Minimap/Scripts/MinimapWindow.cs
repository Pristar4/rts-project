#region Info
// -----------------------------------------------------------------------
// MinimapWindow.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

namespace Minimap
{

	public class MinimapWindow : MonoBehaviour
	{

		private static MinimapWindow instance;

		private void Awake()
		{
			instance = this;
		}

		public static event EventHandler OnWindowShow;
		public static event EventHandler OnWindowHide;

		public static void Show()
		{
			instance.gameObject.SetActive(true);
			if (OnWindowShow != null) OnWindowShow(instance, EventArgs.Empty);
		}

		public static void Hide()
		{
			instance.gameObject.SetActive(false);
			if (OnWindowHide != null) OnWindowHide(instance, EventArgs.Empty);
		}
	}

}
