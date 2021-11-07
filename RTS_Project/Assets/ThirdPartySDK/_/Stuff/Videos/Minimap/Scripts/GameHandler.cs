#region Info
// -----------------------------------------------------------------------
// GameHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey;
using Minimap;
using UnityEngine;
#endregion
public class GameHandler : MonoBehaviour
{

	[SerializeField] private MinimapIcon playerMinimapIcon;

	private void Start()
	{
		Minimap.Minimap.Init();

		CMDebug.ButtonUI(new Vector2(300, 200), "Show Minimap",
				Minimap.Minimap.ShowWindow);
		CMDebug.ButtonUI(new Vector2(300, 160), "Hide Minimap",
				Minimap.Minimap.HideWindow);

		CMDebug.ButtonUI(new Vector2(300, 120), "Player Icon Show",
				playerMinimapIcon.Show);
		CMDebug.ButtonUI(new Vector2(300, 80), "Player Icon Hide",
				playerMinimapIcon.Hide);

		CMDebug.ButtonUI(new Vector2(300, 20), "Zoom In",
				Minimap.Minimap.ZoomIn);
		CMDebug.ButtonUI(new Vector2(300, -20), "Zoom Out",
				Minimap.Minimap.ZoomOut);
	}
}
