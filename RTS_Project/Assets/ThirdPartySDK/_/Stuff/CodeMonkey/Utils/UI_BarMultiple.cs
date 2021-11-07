#region Info
// -----------------------------------------------------------------------
// UI_BarMultiple.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#endregion

namespace CodeMonkey.Utils
{

	/*
	 * UI Container with multiple bars, useful for displaying one bar with multiple inner bars like success chance and failure chance
	 * */
	public class UI_BarMultiple
	{
		private readonly RectTransform[] barArr;
		private readonly Vector2 size;
		private Image[] barImageArr;

		private GameObject gameObject;
		private RectTransform rectTransform;

		public UI_BarMultiple(Transform parent, Vector2 anchoredPosition,
		                      Vector2 size, Color[] barColorArr,
		                      Outline outline)
		{
			this.size = size;
			SetupParent(parent, anchoredPosition, size);
			if (outline != null) SetupOutline(outline, size);
			var barList = new List<RectTransform>();
			var barImageList = new List<Image>();
			var defaultSizeList = new List<float>();
			foreach (var color in barColorArr)
			{
				barList.Add(SetupBar(color));
				defaultSizeList.Add(1f / barColorArr.Length);
			}
			barArr = barList.ToArray();
			barImageArr = barImageList.ToArray();
			SetSizes(defaultSizeList.ToArray());
		}
		private void SetupParent(Transform parent, Vector2 anchoredPosition,
		                         Vector2 size)
		{
			gameObject
					= new GameObject("UI_BarMultiple", typeof(RectTransform));
			rectTransform = gameObject.GetComponent<RectTransform>();
			rectTransform.SetParent(parent, false);
			rectTransform.sizeDelta = size;
			rectTransform.anchorMin = new Vector2(0, .5f);
			rectTransform.anchorMax = new Vector2(0, .5f);
			rectTransform.pivot = new Vector2(0, .5f);
			rectTransform.anchoredPosition = anchoredPosition;
		}
		private void SetupOutline(Outline outline, Vector2 size)
		{
			UtilsClass.DrawSprite(outline.color, gameObject.transform,
					Vector2.zero,
					size + new Vector2(outline.size, outline.size), "Outline");
		}
		private RectTransform SetupBar(Color barColor)
		{
			var bar = UtilsClass.DrawSprite(barColor, gameObject.transform,
					Vector2.zero, Vector2.zero, "Bar");
			bar.anchorMin = new Vector2(0, 0);
			bar.anchorMax = new Vector2(0, 1f);
			bar.pivot = new Vector2(0, .5f);
			return bar;
		}
		public void SetSizes(float[] sizeArr)
		{
			if (sizeArr.Length != barArr.Length)
				throw new Exception("Length doesn't match!");
			var pos = Vector2.zero;
			for (var i = 0; i < sizeArr.Length; i++)
			{
				var scaledSize = sizeArr[i] * size.x;
				barArr[i].anchoredPosition = pos;
				barArr[i].sizeDelta = new Vector2(scaledSize, 0f);
				pos.x += scaledSize;
			}
		}
		public Vector2 GetSize()
		{
			return size;
		}
		public void DestroySelf()
		{
			Object.Destroy(gameObject);
		}

		public class Outline
		{
			public Color color = Color.black;
			public float size = 1f;
			public Outline(float size, Color color)
			{
				this.size = size;
				this.color = color;
			}
		}
	}
}
