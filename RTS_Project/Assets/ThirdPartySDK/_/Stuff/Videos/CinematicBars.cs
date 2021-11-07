#region Info
// -----------------------------------------------------------------------
// CinematicBars.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class CinematicBars : MonoBehaviour
{

	private static CinematicBars instance;
	private float changeSizeAmount;
	private bool isActive;
	private float targetSize;

	private RectTransform topBar, bottomBar;

	private void Awake()
	{
		instance = this;
		var gameObject = new GameObject("topBar", typeof(Image));
		gameObject.transform.SetParent(transform, false);
		gameObject.GetComponent<Image>().color = Color.black;
		topBar = gameObject.GetComponent<RectTransform>();
		topBar.anchorMin = new Vector2(0, 1);
		topBar.anchorMax = new Vector2(1, 1);
		topBar.sizeDelta = new Vector2(0, 0);

		gameObject = new GameObject("bottomBar", typeof(Image));
		gameObject.transform.SetParent(transform, false);
		gameObject.GetComponent<Image>().color = Color.black;
		bottomBar = gameObject.GetComponent<RectTransform>();
		bottomBar.anchorMin = new Vector2(0, 0);
		bottomBar.anchorMax = new Vector2(1, 0);
		bottomBar.sizeDelta = new Vector2(0, 0);

		CMDebug.ButtonUI(new Vector2(-200, 0), "Show",
				() => { Show(300, .3f); });
		CMDebug.ButtonUI(new Vector2(200, 0), "Hide", () => { Hide(.3f); });
	}

	private void Update()
	{
		if (isActive)
		{
			var sizeDelta = topBar.sizeDelta;
			sizeDelta.y += changeSizeAmount * Time.deltaTime;
			if (changeSizeAmount > 0)
			{
				if (sizeDelta.y >= targetSize)
				{
					sizeDelta.y = targetSize;
					isActive = false;
				}
			}
			else
			{
				if (sizeDelta.y <= targetSize)
				{
					sizeDelta.y = targetSize;
					isActive = false;
				}
			}
			topBar.sizeDelta = sizeDelta;
			bottomBar.sizeDelta = sizeDelta;
		}
	}

	public void Show(float targetSize, float time)
	{
		this.targetSize = targetSize;
		changeSizeAmount = (targetSize - topBar.sizeDelta.y) / time;
		isActive = true;
	}

	public void Hide(float time)
	{
		targetSize = 0f;
		changeSizeAmount = (targetSize - topBar.sizeDelta.y) / time;
		isActive = true;
	}


	public static void Show_Static(float targetSize, float time)
	{
		if (instance != null) instance.Show(targetSize, time);
	}

	public static void Hide_Static(float time)
	{
		if (instance != null) instance.Hide(time);
	}
}
