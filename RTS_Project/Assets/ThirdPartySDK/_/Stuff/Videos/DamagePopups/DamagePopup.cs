#region Info
// -----------------------------------------------------------------------
// DamagePopup.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
#endregion
public class DamagePopup : MonoBehaviour
{

	private const float DISAPPEAR_TIMER_MAX = 1f;

	private static int sortingOrder;
	private float disappearTimer;
	private Vector3 moveVector;
	private Color textColor;

	private TextMeshPro textMesh;

	private void Awake()
	{
		textMesh = transform.GetComponent<TextMeshPro>();
	}

	private void Update()
	{
		transform.position += moveVector * Time.deltaTime;
		moveVector -= moveVector * 8f * Time.deltaTime;

		if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
		{
			// First half of the popup lifetime
			var increaseScaleAmount = 1f;
			transform.localScale
					+= Vector3.one * increaseScaleAmount * Time.deltaTime;
		}
		else
		{
			// Second half of the popup lifetime
			var decreaseScaleAmount = 1f;
			transform.localScale
					-= Vector3.one * decreaseScaleAmount * Time.deltaTime;
		}

		disappearTimer -= Time.deltaTime;
		if (disappearTimer < 0)
		{
			// Start disappearing
			var disappearSpeed = 3f;
			textColor.a -= disappearSpeed * Time.deltaTime;
			textMesh.color = textColor;
			if (textColor.a < 0) Destroy(gameObject);
		}
	}

	// Create a Damage Popup
	public static DamagePopup Create(Vector3 position, int damageAmount,
	                                 bool isCriticalHit)
	{
		var damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup,
				position,
				Quaternion.identity);

		var damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
		damagePopup.Setup(damageAmount, isCriticalHit);

		return damagePopup;
	}

	public void Setup(int damageAmount, bool isCriticalHit)
	{
		textMesh.SetText(damageAmount.ToString());
		if (!isCriticalHit)
		{
			// Normal hit
			textMesh.fontSize = 36;
			textColor = UtilsClass.GetColorFromString("FFC500");
		}
		else
		{
			// Critical hit
			textMesh.fontSize = 45;
			textColor = UtilsClass.GetColorFromString("FF2B00");
		}
		textColor.g += Random.Range(-.1f, +.1f);
		textMesh.color = textColor;

		disappearTimer = DISAPPEAR_TIMER_MAX;

		sortingOrder++;
		textMesh.sortingOrder = sortingOrder;

		moveVector = new Vector3(.7f, 1) * 60f;
	}
}
