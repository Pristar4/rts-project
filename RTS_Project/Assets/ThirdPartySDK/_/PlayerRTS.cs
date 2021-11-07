#region Info
// -----------------------------------------------------------------------
// PlayerRTS.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class PlayerRTS : MonoBehaviour
{

	private CharacterRTS selectedCharacter;

	public static PlayerRTS Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var collider2DArray
					= Physics2D.OverlapPointAll(
							UtilsClass.GetMouseWorldPosition());

			// Deselect Character
			SetSelectedCharacter(null);

			foreach (var collider2D in collider2DArray)
			{
				var characterRTS = collider2D.GetComponent<CharacterRTS>();
				if (characterRTS != null && characterRTS.IsPlayer())
					SetSelectedCharacter(characterRTS);
			}
		}

		if (Input.GetMouseButtonDown(1))
			if (selectedCharacter != null)
			{
				var collider2DArray
						= Physics2D.OverlapPointAll(
								UtilsClass.GetMouseWorldPosition());

				var doMoveAction = true;

				foreach (var collider2D in collider2DArray)
				{
					var characterRTS = collider2D.GetComponent<CharacterRTS>();
					if (characterRTS != null && !characterRTS.IsPlayer())
					{
						selectedCharacter.SetTarget(characterRTS);
						doMoveAction = false;
						break;
					}
				}

				if (doMoveAction)
				{
					selectedCharacter.SetTarget(null);
					selectedCharacter.SetMovePosition(
							UtilsClass.GetMouseWorldPosition());
				}
			}
	}

	public void SetSelectedCharacter(CharacterRTS selectedCharacter)
	{
		this.selectedCharacter?.SetSelectedGameObjectVisible(false);

		this.selectedCharacter = selectedCharacter;

		this.selectedCharacter?.SetSelectedGameObjectVisible(true);
	}
}
