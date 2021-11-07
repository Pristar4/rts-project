#region Info
// -----------------------------------------------------------------------
// HotkeyBar_Testing.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class HotkeyBar_Testing : MonoBehaviour
{

	//[SerializeField] private PlayerSwapWeapons player;
	[SerializeField] private UI_HotkeyBar uiHotkeyBar;

	public Sprite pistolSprite;
	public Sprite shotgunSprite;
	public Sprite swordSprite;
	public Sprite punchSprite;
	public Sprite healthPotionSprite;
	public Sprite manaPotionSprite;

	private HotkeyAbilitySystem hotkeyAbilitySystem;

	public static HotkeyBar_Testing Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		hotkeyAbilitySystem = new HotkeyAbilitySystem();
		uiHotkeyBar.SetHotkeyAbilitySystem(hotkeyAbilitySystem);
	}

	private void Update()
	{
		hotkeyAbilitySystem.Update();
	}
}
