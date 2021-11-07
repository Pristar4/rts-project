#region Info
// -----------------------------------------------------------------------
// UI_Controls.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion

namespace TopDownShooter
{
	public class UI_Controls : MonoBehaviour
	{

		public static UI_Controls Instance { get; private set; }

		private void Awake()
		{
			Instance = this;

			GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
			GetComponent<RectTransform>().sizeDelta = Vector3.zero;

			transform.Find("continueBtn").GetComponent<Button_UI>().ClickFunc
					= ()
							=>
					{
						Hide();
						GameHandler_Setup.Instance.ResumeGame();
					};
		}

		private void Start()
		{
			Show();
			GameHandler_Setup.Instance.PauseGame();
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		public void Show()
		{
			gameObject.SetActive(true);
		}
	}
}
