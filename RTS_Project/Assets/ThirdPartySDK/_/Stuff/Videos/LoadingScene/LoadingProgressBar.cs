#region Info
// -----------------------------------------------------------------------
// LoadingProgressBar.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using UnityEngine.UI;
#endregion
public class LoadingProgressBar : MonoBehaviour
{

	private Image image;

	private void Awake()
	{
		image = transform.GetComponent<Image>();
	}

	private void Update()
	{
		image.fillAmount = Loader.GetLoadingProgress();
	}
}
