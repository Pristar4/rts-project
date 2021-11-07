﻿#region Info
// -----------------------------------------------------------------------
// DragDrop.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using UnityEngine.EventSystems;
#endregion
public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,
		IEndDragHandler, IDragHandler
{

	[SerializeField] private Canvas canvas;
	private CanvasGroup canvasGroup;

	private RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		Debug.Log("OnBeginDrag");
		canvasGroup.alpha = .6f;
		canvasGroup.blocksRaycasts = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
		//Debug.Log("OnDrag");
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log("OnEndDrag");
		canvasGroup.alpha = 1f;
		canvasGroup.blocksRaycasts = true;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("OnPointerDown");
	}
}
