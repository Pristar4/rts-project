#region Info
// -----------------------------------------------------------------------
// UI_Keys.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace TopDownShooter
{
	public class UI_Keys : MonoBehaviour
	{

		[SerializeField] private KeyHolder keyHolder;

		private Image keyImage;

		private void Awake()
		{
			keyImage = transform.Find("key").Find("image")
					.GetComponent<Image>();
			keyImage.gameObject.SetActive(false);
		}

		private void Start()
		{
			keyHolder.OnKeysChanged += KeyHolder_OnKeysChanged;
		}

		private void KeyHolder_OnKeysChanged(object sender, EventArgs e)
		{
			var keyList = keyHolder.GetKeyList();
			if (keyList.Count >= 1)
			{
				keyImage.gameObject.SetActive(true);
				keyImage.color = Key.GetColor(keyList[0]);
			}
			else { keyImage.gameObject.SetActive(false); }
		}
	}
}
