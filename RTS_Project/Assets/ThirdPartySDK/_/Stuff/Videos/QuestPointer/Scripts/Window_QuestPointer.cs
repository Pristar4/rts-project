#region Info
// -----------------------------------------------------------------------
// Window_QuestPointer.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class Window_QuestPointer : MonoBehaviour
{

	[SerializeField] private Camera uiCamera;
	[SerializeField] private Sprite arrowSprite;
	[SerializeField] private Sprite crossSprite;

	private List<QuestPointer> questPointerList;

	private void Awake()
	{
		questPointerList = new List<QuestPointer>();
	}

	private void Update()
	{
		foreach (var questPointer in questPointerList) questPointer.Update();
	}

	public QuestPointer CreatePointer(Vector3 targetPosition, Color arrowColor,
	                                  Color crossColor,
	                                  Sprite arrowSprite = null,
	                                  Sprite crossSprite = null)
	{
		if (arrowSprite == null) arrowSprite = this.arrowSprite;
		if (crossSprite == null) crossSprite = this.crossSprite;
		var pointerGameObject
				= Instantiate(transform.Find("pointerTemplate").gameObject);
		pointerGameObject.SetActive(true);
		pointerGameObject.transform.SetParent(transform, false);
		var questPointer = new QuestPointer(targetPosition, pointerGameObject,
				uiCamera, arrowSprite, crossSprite, arrowColor, crossColor);
		questPointerList.Add(questPointer);
		return questPointer;
	}

	public void DestroyPointer(QuestPointer questPointer)
	{
		questPointerList.Remove(questPointer);
		questPointer.DestroySelf();
	}

	public class QuestPointer
	{
		private readonly Color arrowColor;
		private readonly Sprite arrowSprite;
		private readonly Color crossColor;
		private readonly Sprite crossSprite;
		private readonly GameObject pointerGameObject;
		private readonly Image pointerImage;
		private readonly RectTransform pointerRectTransform;

		private readonly Vector3 targetPosition;
		private readonly Camera uiCamera;

		public QuestPointer(Vector3 targetPosition,
		                    GameObject pointerGameObject,
		                    Camera uiCamera, Sprite arrowSprite,
		                    Sprite crossSprite,
		                    Color arrowColor, Color crossColor)
		{
			this.targetPosition = targetPosition;
			this.pointerGameObject = pointerGameObject;
			this.uiCamera = uiCamera;
			this.arrowSprite = arrowSprite;
			this.crossSprite = crossSprite;
			this.arrowColor = arrowColor;
			this.crossColor = crossColor;

			pointerRectTransform
					= pointerGameObject.GetComponent<RectTransform>();
			pointerImage = pointerGameObject.GetComponent<Image>();
		}

		public void Update()
		{
			var borderSize = 100f;
			var targetPositionScreenPoint
					= Camera.main.WorldToScreenPoint(targetPosition);
			var isOffScreen = targetPositionScreenPoint.x <= borderSize ||
			                  targetPositionScreenPoint.x >=
			                  Screen.width - borderSize ||
			                  targetPositionScreenPoint.y <= borderSize ||
			                  targetPositionScreenPoint.y >=
			                  Screen.height - borderSize;

			if (isOffScreen)
			{
				RotatePointerTowardsTargetPosition();

				pointerImage.sprite = arrowSprite;
				pointerImage.color = arrowColor;
				var cappedTargetScreenPosition = targetPositionScreenPoint;
				cappedTargetScreenPosition.x = Mathf.Clamp(
						cappedTargetScreenPosition.x,
						borderSize, Screen.width - borderSize);
				cappedTargetScreenPosition.y = Mathf.Clamp(
						cappedTargetScreenPosition.y,
						borderSize, Screen.height - borderSize);

				var pointerWorldPosition
						= uiCamera.ScreenToWorldPoint(
								cappedTargetScreenPosition);
				pointerRectTransform.position = pointerWorldPosition;
				pointerRectTransform.localPosition = new Vector3(
						pointerRectTransform.localPosition.x,
						pointerRectTransform.localPosition.y, 0f);
			}
			else
			{
				pointerImage.sprite = crossSprite;
				pointerImage.color = crossColor;
				var pointerWorldPosition
						= uiCamera.ScreenToWorldPoint(
								targetPositionScreenPoint);
				pointerRectTransform.position = pointerWorldPosition;
				pointerRectTransform.localPosition = new Vector3(
						pointerRectTransform.localPosition.x,
						pointerRectTransform.localPosition.y, 0f);

				pointerRectTransform.localEulerAngles = Vector3.zero;
			}
		}

		private void RotatePointerTowardsTargetPosition()
		{
			var toPosition = targetPosition;
			var fromPosition = Camera.main.transform.position;
			fromPosition.z = 0f;
			var dir = (toPosition - fromPosition).normalized;
			var angle = UtilsClass.GetAngleFromVectorFloat(dir);
			pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
		}

		public void DestroySelf()
		{
			Destroy(pointerGameObject);
		}
	}
}
