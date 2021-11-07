#region Info
// -----------------------------------------------------------------------
// DragWindow.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using UnityEngine.EventSystems;
#endregion
public class DragWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{

	[SerializeField] private RectTransform dragRectTransform;
	[SerializeField] private Canvas canvas;

	private void Awake()
	{
		if (dragRectTransform == null)
			dragRectTransform = transform.parent.GetComponent<RectTransform>();

		if (canvas == null)
		{
			var testCanvasTransform = transform.parent;
			while (testCanvasTransform != null)
			{
				canvas = testCanvasTransform.GetComponent<Canvas>();
				if (canvas != null) break;
				testCanvasTransform = testCanvasTransform.parent;
			}
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		dragRectTransform.anchoredPosition
				+= eventData.delta / canvas.scaleFactor;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		dragRectTransform.SetAsLastSibling();
	}
}
