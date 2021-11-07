#region Info
// -----------------------------------------------------------------------
// Testing_BattleSystem.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion
public class Testing_BattleSystem : MonoBehaviour
{

	[SerializeField] private DoorAnims entryDoor;
	[SerializeField] private DoorAnims exitDoor;
	[SerializeField] private BattleSystem battleSystem;

	private void Start()
	{
		battleSystem.OnBattleStarted += BattleSystem_OnBattleStarted;
		battleSystem.OnBattleOver += BattleSystem_OnBattleOver;
	}

	private void BattleSystem_OnBattleOver(object sender, EventArgs e)
	{
		exitDoor.OpenDoor();
		exitDoor.SetColor(DoorAnims.ColorName.Green);
	}

	private void BattleSystem_OnBattleStarted(object sender, EventArgs e)
	{
		entryDoor.CloseDoor();
		entryDoor.SetColor(DoorAnims.ColorName.Red);
	}
}
