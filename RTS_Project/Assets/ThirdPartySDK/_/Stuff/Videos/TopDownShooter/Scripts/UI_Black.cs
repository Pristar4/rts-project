#region Info
// -----------------------------------------------------------------------
// UI_Black.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace TopDownShooter
{
	public class UI_Black : MonoBehaviour
	{

		private Image blackImage;
		private Color color;

		private void Awake()
		{
			GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
			GetComponent<RectTransform>().sizeDelta = Vector3.zero;

			blackImage = transform.Find("blackImage").GetComponent<Image>();
			color = new Color(0, 0, 0, 1f);
			blackImage.color = color;
		}

		private void Update()
		{
			var fadeSpeed = 2f;
			color.a -= fadeSpeed * Time.deltaTime;

			blackImage.color = color;

			if (color.a <= 0f) gameObject.SetActive(false);
		}
	}
}
