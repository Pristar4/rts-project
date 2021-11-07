#region Info
// -----------------------------------------------------------------------
// Tooltip_ItemStats.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class Tooltip_ItemStats : MonoBehaviour
{

	private static Tooltip_ItemStats instance;

	[SerializeField]
	private Camera uiCamera;
	[SerializeField]
	private RectTransform canvasRectTransform;
	private RectTransform backgroundRectTransform;
	private Text descriptionText;

	private Image image;
	private Text levelText;
	private Text nameText;

	private void Awake()
	{
		instance = this;
		backgroundRectTransform
				= transform.Find("background").GetComponent<RectTransform>();
		image = transform.Find("image").GetComponent<Image>();
		nameText = transform.Find("nameText").GetComponent<Text>();
		descriptionText
				= transform.Find("descriptionText").GetComponent<Text>();
		levelText = transform.Find("levelText").GetComponent<Text>();

		HideTooltip();
	}

	private void Update()
	{
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
				transform.parent.GetComponent<RectTransform>(),
				Input.mousePosition,
				uiCamera, out localPoint);
		transform.localPosition = localPoint;

		var anchoredPosition
				= transform.GetComponent<RectTransform>().anchoredPosition;
		if (anchoredPosition.x + backgroundRectTransform.rect.width >
		    canvasRectTransform.rect.width)
			anchoredPosition.x = canvasRectTransform.rect.width -
			                     backgroundRectTransform.rect.width;
		if (anchoredPosition.y - backgroundRectTransform.rect.height >
		    canvasRectTransform.rect.height)
			anchoredPosition.y = canvasRectTransform.rect.height +
			                     backgroundRectTransform.rect.height;
		transform.GetComponent<RectTransform>().anchoredPosition
				= anchoredPosition;
	}

	private void ShowTooltip(Sprite itemSprite, string itemName,
	                         string itemDescription, int level)
	{
		gameObject.SetActive(true);
		transform.SetAsLastSibling();
		nameText.text = itemName;
		descriptionText.text = itemDescription;
		levelText.text = level.ToString();
		image.sprite = itemSprite;
		Update();
	}

	private void HideTooltip()
	{
		gameObject.SetActive(false);
	}

	public static void ShowTooltip_Static(Sprite itemSprite, string itemName,
	                                      string itemDescription, int level)
	{
		instance.ShowTooltip(itemSprite, itemName, itemDescription, level);
	}

	public static void HideTooltip_Static()
	{
		instance.HideTooltip();
	}





	public static void AddTooltip(Transform transform, Sprite itemSprite,
	                              string itemName, string itemDescription,
	                              int level)
	{
		if (transform.GetComponent<Button_UI>() != null)
		{
			transform.GetComponent<Button_UI>().MouseOverOnceTooltipFunc = ()
					=> ShowTooltip_Static(itemSprite, itemName, itemDescription,
							level);
			transform.GetComponent<Button_UI>().MouseOutOnceTooltipFunc
					= () => HideTooltip_Static();
		}
	}
}
