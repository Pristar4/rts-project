#region Info
// -----------------------------------------------------------------------
// Tooltip_Warning.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class Tooltip_Warning : MonoBehaviour
{

	private static Tooltip_Warning instance;

	[SerializeField]
	private Camera uiCamera;
	[SerializeField]
	private RectTransform canvasRectTransform;
	private Image backgroundImage;
	private RectTransform backgroundRectTransform;
	private int flashState;
	private float flashTimer;
	private Func<string> getTooltipStringFunc;
	private float showTimer;

	private Text tooltipText;

	private void Awake()
	{
		instance = this;
		backgroundRectTransform
				= transform.Find("background").GetComponent<RectTransform>();
		tooltipText = transform.Find("text").GetComponent<Text>();
		backgroundImage = transform.Find("background").GetComponent<Image>();

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

		SetText(getTooltipStringFunc());

		var anchoredPosition
				= transform.GetComponent<RectTransform>().anchoredPosition;
		if (anchoredPosition.x + backgroundRectTransform.rect.width >
		    canvasRectTransform.rect.width)
			anchoredPosition.x = canvasRectTransform.rect.width -
			                     backgroundRectTransform.rect.width;
		if (anchoredPosition.y + backgroundRectTransform.rect.height >
		    canvasRectTransform.rect.height)
			anchoredPosition.y = canvasRectTransform.rect.height -
			                     backgroundRectTransform.rect.height;
		transform.GetComponent<RectTransform>().anchoredPosition
				= anchoredPosition;

		flashTimer += Time.deltaTime;
		var flashTimerMax = .033f;
		if (flashTimer > flashTimerMax)
		{
			flashTimer = 0f;
			flashState++;
			switch (flashState)
			{
				case 1:
				case 3:
				case 5:
					tooltipText.color = new Color(1, 1, 1, 1);
					backgroundImage.color = new Color(178f / 255f, 0 / 255f,
							0 / 255f, 1);
					break;
				case 2:
				case 4:
					tooltipText.color
							= new Color(178f / 255f, 0 / 255f, 0 / 255f, 1);
					backgroundImage.color = new Color(1, 1, 1, 1);
					break;
			}
		}

		showTimer -= Time.deltaTime;
		if (showTimer <= 0f) HideTooltip();
	}

	private void ShowTooltip(string tooltipString, float showTimerMax = 2f)
	{
		ShowTooltip(() => tooltipString, showTimerMax);
	}

	private void ShowTooltip(Func<string> getTooltipStringFunc,
	                         float showTimerMax = 2f)
	{
		gameObject.SetActive(true);
		transform.SetAsLastSibling();
		this.getTooltipStringFunc = getTooltipStringFunc;
		showTimer = showTimerMax;
		flashTimer = 0f;
		flashState = 0;
		Update();
	}

	private void SetText(string tooltipString)
	{
		tooltipText.text = tooltipString;
		var textPaddingSize = 4f;
		var backgroundSize
				= new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f,
						tooltipText.preferredHeight + textPaddingSize * 2f);
		backgroundRectTransform.sizeDelta = backgroundSize;
	}

	private void HideTooltip()
	{
		gameObject.SetActive(false);
	}

	public static void ShowTooltip_Static(string tooltipString)
	{
		instance.ShowTooltip(tooltipString);
	}

	public static void ShowTooltip_Static(Func<string> getTooltipStringFunc)
	{
		instance.ShowTooltip(getTooltipStringFunc);
	}

	public static void HideTooltip_Static()
	{
		instance.HideTooltip();
	}
}
