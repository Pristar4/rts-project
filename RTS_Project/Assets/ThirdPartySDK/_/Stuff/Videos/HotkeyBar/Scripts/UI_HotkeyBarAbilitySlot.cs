#region Info
// -----------------------------------------------------------------------
// UI_HotkeyBarAbilitySlot.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using UnityEngine.EventSystems;
#endregion
public class UI_HotkeyBarAbilitySlot : MonoBehaviour, IPointerDownHandler,
		IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler
{
	private int abilityIndex;
	private Canvas canvas;
	private CanvasGroup canvasGroup;
	private HotkeyAbilitySystem.HotkeyAbility hotkeyAbility;
	private HotkeyAbilitySystem hotkeySystem;

	private RectTransform rectTransform;

	private Vector2 startAnchoredPosition;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();

		// Automatically grab Canvas
		var testCanvasTransform = transform;
		do
		{
			testCanvasTransform = testCanvasTransform.parent;
			canvas = testCanvasTransform.GetComponent<Canvas>();
		} while (canvas == null);
	}

	private void Start()
	{
		startAnchoredPosition = rectTransform.anchoredPosition;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		canvasGroup.alpha = .6f;
		canvasGroup.blocksRaycasts = false;
		transform.SetAsLastSibling();
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnDrop(PointerEventData eventData)
	{
		if (eventData.pointerDrag != null)
		{
			// Dragging something
			var uiHotkeyBarAbilitySlot
					= eventData.pointerDrag
							.GetComponent<UI_HotkeyBarAbilitySlot>();
			if (uiHotkeyBarAbilitySlot !=
			    null) // Dragging Slot and dropped on this one
					//hotkeySystem.SwapAbility(abilityIndex, uiHotkeyBarAbilitySlot.GetAbilityIndex());
				hotkeySystem.SwapAbility(hotkeyAbility,
						uiHotkeyBarAbilitySlot.GetHotkeyAbility());
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition = startAnchoredPosition;
		canvasGroup.alpha = 1f;
		canvasGroup.blocksRaycasts = true;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		hotkeyAbility.activateAbilityAction();
	}

	public int GetAbilityIndex()
	{
		return abilityIndex;
	}

	public HotkeyAbilitySystem.HotkeyAbility GetHotkeyAbility()
	{
		return hotkeyAbility;
	}

	public void Setup(HotkeyAbilitySystem hotkeySystem, int abilityIndex,
	                  HotkeyAbilitySystem.HotkeyAbility hotkeyAbility)
	{
		this.hotkeySystem = hotkeySystem;
		this.abilityIndex = abilityIndex;
		this.hotkeyAbility = hotkeyAbility;
	}
}
