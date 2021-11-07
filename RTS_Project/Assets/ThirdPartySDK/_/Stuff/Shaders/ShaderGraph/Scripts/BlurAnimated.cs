#region Info
// -----------------------------------------------------------------------
// BlurAnimated.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class BlurAnimated : MonoBehaviour
{

	[SerializeField] private Material material;
	private bool blurActive;

	private float blurAmount;

	private void Start()
	{
		blurAmount = 0;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T)) blurActive = !blurActive;

		var blurSpeed = 15f;
		if (blurActive)
			blurAmount += blurSpeed * Time.deltaTime;
		else
			blurAmount -= blurSpeed * Time.deltaTime;

		blurAmount = Mathf.Clamp(blurAmount, 0f, 4f);
		material.SetFloat("_BlurAmount", blurAmount);
	}
}
