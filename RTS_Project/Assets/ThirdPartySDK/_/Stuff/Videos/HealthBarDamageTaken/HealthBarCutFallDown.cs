#region Info
// -----------------------------------------------------------------------
// HealthBarCutFallDown.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using UnityEngine.UI;
#endregion
public class HealthBarCutFallDown : MonoBehaviour
{
	private Color color;
	private float fadeTimer;
	private float fallDownTimer;
	private Image image;

	private RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = transform.GetComponent<RectTransform>();
		image = transform.GetComponent<Image>();
		color = image.color;
		fallDownTimer = .6f;
		fadeTimer = .5f;
	}

	private void Update()
	{
		fallDownTimer -= Time.deltaTime;
		if (fallDownTimer < 0)
		{
			var fallSpeed = 100f;
			rectTransform.anchoredPosition
					+= Vector2.down * fallSpeed * Time.deltaTime;

			fadeTimer -= Time.deltaTime;
			if (fadeTimer < 0)
			{
				var alphaFadeSpeed = 5f;
				color.a -= alphaFadeSpeed * Time.deltaTime;
				image.color = color;

				if (color.a <= 0) Destroy(gameObject);
			}
		}
	}
}
