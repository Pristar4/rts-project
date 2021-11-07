#region Info
// -----------------------------------------------------------------------
// EnableLights.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
#endregion


/*
 * Enable Lights when Player enters trigger
 * */
public class EnableLights : MonoBehaviour
{

	[SerializeField] private CaptureOnTriggerEnter2D enableLightsTrigger;
	[SerializeField] private Light2D[] lightArray;
	[SerializeField] private float targetLightIntensity;
	[SerializeField] private float lightIntensitySpeed;

	private float lightIntensity;

	private void Start()
	{
		if (enableLightsTrigger != null)
			enableLightsTrigger.OnPlayerTriggerEnter2D
					+= EnableLightsTrigger_OnPlayerTriggerEnter2D;

		foreach (var light in lightArray) light.intensity = 0f;

		enabled = false;
	}

	private void Update()
	{
		lightIntensity += lightIntensitySpeed * Time.deltaTime;
		lightIntensity = Mathf.Clamp(lightIntensity, 0f, targetLightIntensity);

		foreach (var light in lightArray) light.intensity = lightIntensity;

		if (lightIntensity >= targetLightIntensity) enabled = false;
	}

	private void EnableLightsTrigger_OnPlayerTriggerEnter2D(
			object sender, EventArgs e)
	{
		TurnLightsOn();
		enableLightsTrigger.OnPlayerTriggerEnter2D
				-= EnableLightsTrigger_OnPlayerTriggerEnter2D;
	}

	public void TurnLightsOn()
	{
		enabled = true;
	}
}
