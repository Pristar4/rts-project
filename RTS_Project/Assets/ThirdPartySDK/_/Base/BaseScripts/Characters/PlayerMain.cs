#region Info
// -----------------------------------------------------------------------
// PlayerMain.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion

/*
 * Player Class References
 * */
public class PlayerMain : MonoBehaviour
{

	//public Player Player { get; private set; }
	public Character_Base CharacterBase { get; private set; }
	public PlayerMovementHandler PlayerMovementHandler { get; private set; }

	public Rigidbody2D PlayerRigidbody2D { get; private set; }

	private void Awake()
	{
		//Player = GetComponent<Player>();
		CharacterBase = GetComponent<Character_Base>();
		PlayerMovementHandler = GetComponent<PlayerMovementHandler>();

		PlayerRigidbody2D = GetComponent<Rigidbody2D>();
	}
}
