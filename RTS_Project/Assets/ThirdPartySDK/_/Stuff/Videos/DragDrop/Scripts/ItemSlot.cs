#region Info
// -----------------------------------------------------------------------
// ItemSlot.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
using UnityEngine.EventSystems;
#endregion
public class ItemSlot : MonoBehaviour, IDropHandler
{

	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("OnDrop");
		if (eventData.pointerDrag != null)
			eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition
					= GetComponent<RectTransform>().anchoredPosition;
	}
}
