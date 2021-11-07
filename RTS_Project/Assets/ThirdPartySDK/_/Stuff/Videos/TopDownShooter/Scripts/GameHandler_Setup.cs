#region Info
// -----------------------------------------------------------------------
// GameHandler_Setup.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.MonoBehaviours;
using CodeMonkey.Utils;
using GridPathfindingSystem;
using UnityEngine;
#endregion

namespace TopDownShooter
{
	public class GameHandler_Setup : MonoBehaviour
	{
		public static GridPathfinding gridPathfinding;

		[SerializeField] private CameraFollow cameraFollow;
		[SerializeField] private Transform followTransform;
		[SerializeField] private bool cameraPositionWithMouse;
		[SerializeField] private UI_Weapon uiWeapon;

		[SerializeField] private Player player;

		[SerializeField] private DoorAnims doorAnims;
		[SerializeField] private Transform pfPathfindingUnWalkable;
		[SerializeField] private Transform pfPathfindingWalkable;

		public static GameHandler_Setup Instance { get; private set; }

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			Sound_Manager.Init();
			cameraFollow.Setup(GetCameraPosition, () => 60f, true, true);

			var pathfindingLowerLeft
					= transform.Find("PathfindingLowerLeft").position;
			var pathfindingUpperRight
					= transform.Find("PathfindingUpperRight").position;

			gridPathfinding
					= new GridPathfinding(pathfindingLowerLeft,
							pathfindingUpperRight,
							5f);
			//gridPathfinding.RaycastWalkable(1 << GameAssets.i.wallLayer);
			//gridPathfinding.PrintMap(pfPathfindingWalkable, pfPathfindingUnWalkable);

			//Enemy enemy = Enemy.Create(player.GetPosition() + new Vector3(+60, 0));
			//enemy.EnemyMain.EnemyTargeting.SetGetTarget(() => player);

			uiWeapon.SetWeapon(player.GetWeapon());
			player.OnWeaponChanged += Player_OnWeaponChanged;

			//FunctionTimer.Create(() => doorAnims.OpenDoor(), 3f);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape) ||
			    Input.GetKeyDown(KeyCode.Tab))
			{
				PauseGame();
				UI_Controls.Instance.Show();
			}
		}

		private void Player_OnWeaponChanged(object sender, EventArgs e)
		{
			uiWeapon.SetWeapon(player.GetWeapon());
		}

		private Vector3 GetCameraPosition()
		{
			if (cameraPositionWithMouse)
			{
				var mousePosition = UtilsClass.GetMouseWorldPosition();
				var playerToMouseDirection
						= mousePosition - followTransform.position;
				return followTransform.position + playerToMouseDirection * .3f;
			}
			return followTransform.position;
		}

		private void SpawnEnemy()
		{
			var enemy
					= Enemy.Create(
							player.GetPosition() +
							UtilsClass.GetRandomDir() * 40f,
							Enemy.EnemyType.Archer);
			enemy.EnemyMain.EnemyTargeting.SetGetTarget(() => player);
		}

		public void PauseGame()
		{
			Time.timeScale = 0f;
		}

		public void ResumeGame()
		{
			Time.timeScale = 1f;
		}
	}

}
