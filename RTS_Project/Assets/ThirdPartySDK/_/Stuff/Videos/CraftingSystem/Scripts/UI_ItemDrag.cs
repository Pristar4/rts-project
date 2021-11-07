#region Info
// -----------------------------------------------------------------------
// UI_ItemDrag.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class UI_ItemDrag : MonoBehaviour
{
	private TextMeshProUGUI amountText;

	private Canvas canvas;
	private CanvasGroup canvasGroup;
	private Image image;
	private Item item;
	private RectTransform parentRectTransform;
	private RectTransform rectTransform;

	public static UI_ItemDrag Instance { get; private set; }

	private void Awake()
	{
		Instance = this;

		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		canvas = GetComponentInParent<Canvas>();
		image = transform.Find("image").GetComponent<Image>();
		amountText = transform.Find("amountText")
				.GetComponent<TextMeshProUGUI>();
		parentRectTransform = transform.parent.GetComponent<RectTransform>();

		Hide();
	}

	private void Update()
	{
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
				parentRectTransform,
				Input.mousePosition, null, out var localPoint);
		transform.localPosition = localPoint;
	}

	public Item GetItem()
	{
		return item;
	}

	public void SetItem(Item item)
	{
		this.item = item;
	}

	public void SetSprite(Sprite sprite)
	{
		image.sprite = sprite;
	}

	public void SetAmountText(int amount)
	{
		if (amount <= 1)
			amountText.text = "";
		else // More than 1
			amountText.text = amount.ToString();
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Show(Item item)
	{
		gameObject.SetActive(true);

		SetItem(item);
		SetSprite(item.GetSprite());
		SetAmountText(item.amount);
		UpdatePosition();
	}
}
