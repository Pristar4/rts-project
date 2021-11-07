#region Info
// -----------------------------------------------------------------------
// MinimapIcon.cs
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

	public class MinimapIcon : MonoBehaviour
	{

		private Vector3 baseScale;

		private void Start()
		{
			baseScale = transform.localScale;
			Minimap.OnZoomChanged += Minimap_OnZoomChanged;
		}

		private void OnDestroy()
		{
			Minimap.OnZoomChanged -= Minimap_OnZoomChanged;
		}

		private void Minimap_OnZoomChanged(object sender, EventArgs e)
		{
			transform.localScale = baseScale * Minimap.GetZoom() / 180f;
		}

		public void Show()
		{
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}

}
