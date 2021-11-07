#region Info
// -----------------------------------------------------------------------
// UI_KeyHolder.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class UI_KeyHolder : MonoBehaviour
{

	[SerializeField] private KeyHolder keyHolder;

	private Transform container;
	private Transform keyTemplate;

	private void Awake()
	{
		container = transform.Find("container");
		keyTemplate = container.Find("keyTemplate");
		keyTemplate.gameObject.SetActive(false);
	}

	private void Start()
	{
		keyHolder.OnKeysChanged += KeyHolder_OnKeysChanged;
	}

	private void KeyHolder_OnKeysChanged(object sender, EventArgs e)
	{
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		// Clean up old keys
		foreach (Transform child in container)
		{
			if (child == keyTemplate) continue;
			Destroy(child.gameObject);
		}

		// Instantiate current key list
		var keyList = keyHolder.GetKeyList();
		container.GetComponent<RectTransform>().anchoredPosition
				= new Vector2(-(keyList.Count - 1) * 80 / 2f, -234);
		for (var i = 0; i < keyList.Count; i++)
		{
			var keyType = keyList[i];
			var keyTransform = Instantiate(keyTemplate, container);
			keyTransform.gameObject.SetActive(true);
			keyTransform.GetComponent<RectTransform>().anchoredPosition
					= new Vector2(80 * i, 0);
			var keyImage = keyTransform.Find("image").GetComponent<Image>();
			switch (keyType)
			{
				default:
				case Key.KeyType.Red:
					keyImage.color = Color.red;
					break;
				case Key.KeyType.Green:
					keyImage.color = Color.green;
					break;
				case Key.KeyType.Blue:
					keyImage.color = Color.blue;
					break;
			}
		}
	}
}
