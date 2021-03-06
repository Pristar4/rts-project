#region Info
// -----------------------------------------------------------------------
// Window.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class Window : MonoBehaviour
{

	private float attackCooldown;

	private void Start()
	{
		/* Using the Extension Method
		 * 
		 * transform.Find("attackBtn").AddTooltip(() => "Attack, " + attackCooldown);
		 * */

		Tooltip.AddTooltip(transform.Find("attackTooltipBtn"), "Attack damage");
		Tooltip.AddTooltip(transform.Find("speedTooltipBtn"), "Movement speed");
		Tooltip.AddTooltip(transform.Find("healthTooltipBtn"), "Health amount");

		Tooltip.AddTooltip(transform.Find("patrolBtn"), "Patrol");
		Tooltip.AddTooltip(transform.Find("defendBtn"), "Defend");

		Tooltip.AddTooltip(transform.Find("attackBtn"),
				() => "Attack, " + Mathf.Round(attackCooldown * 100f) / 100f);

		transform.Find("attackBtn").GetComponent<Button_UI>().ClickFunc = () =>
		{
			if (attackCooldown > 0)
				Tooltip_Warning.ShowTooltip_Static("Cannot attack!");
			else
				attackCooldown = 5f;
		};
	}

	private void Update()
	{
		attackCooldown -= Time.deltaTime;
		if (attackCooldown < 0) attackCooldown = 0f;
	}
}
