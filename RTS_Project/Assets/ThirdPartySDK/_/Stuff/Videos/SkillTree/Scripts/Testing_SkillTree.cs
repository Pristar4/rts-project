#region Info
// -----------------------------------------------------------------------
// Testing_SkillTree.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class Testing_SkillTree : MonoBehaviour
{

	[SerializeField] private Player player;
	[SerializeField] private UI_SkillTree uiSkillTree;

	private void Start()
	{
		//uiSkillTree.SetPlayerSkills(player.GetPlayerSkills());
	}
}
