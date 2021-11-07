#region Info
// -----------------------------------------------------------------------
// BossBattleInteraction.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion
public class BossBattleInteraction : MonoBehaviour
{

	[SerializeField] private DoorAnims entryDoor;
	[SerializeField] private GameObject keyGameObject;
	[SerializeField] private BossBattle bossBattle;

	private void Start()
	{
		bossBattle.OnBossBattleStarted += BossBattle_OnBossBattleStarted;
		bossBattle.OnBossBattleOver += BossBattle_OnBossBattleOver;
		keyGameObject.SetActive(false);
	}

	private void BossBattle_OnBossBattleOver(object sender, EventArgs e)
	{
		keyGameObject.SetActive(true);
	}

	private void BossBattle_OnBossBattleStarted(object sender, EventArgs e)
	{
		entryDoor.CloseDoor();
		entryDoor.SetColor(DoorAnims.ColorName.Red);
	}
}
