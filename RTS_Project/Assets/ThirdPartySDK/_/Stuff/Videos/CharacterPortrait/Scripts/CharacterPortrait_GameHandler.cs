#region Info
// -----------------------------------------------------------------------
// CharacterPortrait_GameHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.MonoBehaviours;
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class CharacterPortrait_GameHandler : MonoBehaviour
{

	public static CharacterPortrait_GameHandler instance;
	public Canvas canvas;
	public Transform pfWindow_CharacterPortrait;

	[SerializeField] private CameraFollow cameraFollow;

	[SerializeField] private Transform characterTransform;
	[SerializeField] private Transform secondCharacterTransform;
	private Vector3 cameraFollowPosition = new Vector3(-134, -50);

	private Character rifleCharacter;
	private Character swordCharacter;

	private void Awake()
	{
		instance = this;
		cameraFollow.Setup(() => cameraFollowPosition, () => 80f, true, true);
		SetupCharacters();
	}

	private void Start()
	{
		characterTransform.GetComponent<Button_Sprite>().ClickFunc = () =>
		{
			Window_CharacterPortrait.Show_Static(rifleCharacter);
		};
		secondCharacterTransform.GetComponent<Button_Sprite>().ClickFunc = () =>
		{
			Window_CharacterPortrait.Show_Static(swordCharacter);
		};
	}

	private void Update()
	{
		HandleCameraMovement();
	}





	private void SetupCharacters()
	{
		rifleCharacter = new Character(characterTransform, "Neo");
		swordCharacter = new Character(secondCharacterTransform, "Leonidas");
	}

	private void HandleCameraMovement()
	{
		var cameraMoveSpeed = 200f;
		if (Input.GetKey(KeyCode.W))
			cameraFollowPosition.y += cameraMoveSpeed * Time.deltaTime;
		if (Input.GetKey(KeyCode.S))
			cameraFollowPosition.y -= cameraMoveSpeed * Time.deltaTime;
		if (Input.GetKey(KeyCode.A))
			cameraFollowPosition.x -= cameraMoveSpeed * Time.deltaTime;
		if (Input.GetKey(KeyCode.D))
			cameraFollowPosition.x += cameraMoveSpeed * Time.deltaTime;
	}
}
