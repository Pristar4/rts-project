#region Info
// -----------------------------------------------------------------------
// ExperienceBar.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using UnityEngine.UI;
#endregion
public class ExperienceBar : MonoBehaviour
{
	private RectTransform barMaskRectTransform;

	private float barMaskWidth;
	private RawImage barRawImage;
	private RectTransform edgeRectTransform;

	private void Awake()
	{
		barMaskRectTransform
				= transform.Find("barMask").GetComponent<RectTransform>();
		barRawImage
				= transform.Find("barMask").Find("bar")
						.GetComponent<RawImage>();
		edgeRectTransform
				= transform.Find("edge").GetComponent<RectTransform>();

		barMaskWidth = barMaskRectTransform.sizeDelta.x;
	}

	private void Start()
	{
		SetSize(0f);
	}

	public void SetSize(float sizeNormalized)
	{
		var uvRect = barRawImage.uvRect;
		uvRect.x += .2f * Time.deltaTime;
		barRawImage.uvRect = uvRect;

		var barMaskSizeDelta = barMaskRectTransform.sizeDelta;
		barMaskSizeDelta.x = sizeNormalized * barMaskWidth;
		barMaskRectTransform.sizeDelta = barMaskSizeDelta;

		edgeRectTransform.anchoredPosition
				= new Vector2(sizeNormalized * barMaskWidth, 0);

		edgeRectTransform.gameObject.SetActive(sizeNormalized < 1f &&
		                                       sizeNormalized > 0f);
	}
}
