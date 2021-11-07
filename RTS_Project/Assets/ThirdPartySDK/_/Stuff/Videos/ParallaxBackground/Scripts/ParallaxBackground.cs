#region Info
// -----------------------------------------------------------------------
// ParallaxBackground.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class ParallaxBackground : MonoBehaviour
{

	[SerializeField] private Vector2 parallaxEffectMultiplier;
	[SerializeField] private bool infiniteHorizontal;
	[SerializeField] private bool infiniteVertical;

	private Transform cameraTransform;
	private Vector3 lastCameraPosition;
	private float textureUnitSizeX;
	private float textureUnitSizeY;

	private void Start()
	{
		cameraTransform = Camera.main.transform;
		lastCameraPosition = cameraTransform.position;
		var sprite = GetComponent<SpriteRenderer>().sprite;
		var texture = sprite.texture;
		textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
		textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
	}

	private void LateUpdate()
	{
		var deltaMovement = cameraTransform.position - lastCameraPosition;
		transform.position
				+= new Vector3(deltaMovement.x * parallaxEffectMultiplier.x,
						deltaMovement.y * parallaxEffectMultiplier.y);
		lastCameraPosition = cameraTransform.position;

		if (infiniteHorizontal)
			if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >=
			    textureUnitSizeX)
			{
				var offsetPositionX
						= (cameraTransform.position.x - transform.position.x) %
						  textureUnitSizeX;
				transform.position
						= new Vector3(
								cameraTransform.position.x + offsetPositionX,
								transform.position.y);
			}

		if (infiniteVertical)
			if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >=
			    textureUnitSizeY)
			{
				var offsetPositionY
						= (cameraTransform.position.y - transform.position.y) %
						  textureUnitSizeY;
				transform.position = new Vector3(transform.position.x,
						cameraTransform.position.y + offsetPositionY);
			}
	}
}
