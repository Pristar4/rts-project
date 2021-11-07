#region Info
// -----------------------------------------------------------------------
// AimingShoot.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class AimingShoot : MonoBehaviour
{

	private float aimAngle;

	private Color aimColor;
	private SpriteRenderer aimLeftSpriteRenderer;
	private Transform aimLeftTransform;
	private SpriteRenderer aimRightSpriteRenderer;
	private Transform aimRightTransform;
	private float aimSpeed;
	private Transform aimTransform;

	private Action shootAction;
	private float shootAngle;

	private float startAimAngle;

	private void Awake()
	{
		aimTransform = transform.Find("Aim");
		aimLeftTransform = aimTransform.Find("Left");
		aimRightTransform = aimTransform.Find("Right");

		aimLeftSpriteRenderer = aimLeftTransform.Find("Sprite")
				.GetComponent<SpriteRenderer>();
		aimRightSpriteRenderer = aimRightTransform.Find("Sprite")
				.GetComponent<SpriteRenderer>();

		aimColor = aimLeftSpriteRenderer.color;
		HideAim();
	}

	public void StartAimingAtTarget(Vector3 targetPosition, float startAimAngle,
	                                float shootAngle, float aimSpeed,
	                                Action shootAction)
	{
		this.startAimAngle = startAimAngle;
		this.shootAngle = shootAngle;
		this.aimSpeed = aimSpeed;
		this.shootAction = shootAction;

		ShowAim();
		//SetAimColor(Color.yellow, .0f, GameAssets.i.m_LineEmissionYellow);
		SetAimColor(Color.yellow, .0f, null);
		SetAimAngle(startAimAngle);
		AimAtPosition(targetPosition);
	}

	public void UpdateAimShootAtTarget(Vector3 targetPosition)
	{
		AimAtPosition(targetPosition);

		SetAimAngle(aimAngle - aimSpeed * Time.deltaTime);

		if (
				IsInsideDodgeAngle()) //SetAimColor(Color.red, Mathf.Lerp(0f, 1.1f, 1 - aimAngle / startAimAngle), GameAssets.i.m_LineEmissionRed);
			SetAimColor(Color.red,
					Mathf.Lerp(0f, 1.1f, 1 - aimAngle / startAimAngle),
					null);
		else //SetAimColor(Color.yellow, Mathf.Lerp(0f, 1.1f, 1 - aimAngle / startAimAngle), GameAssets.i.m_LineEmissionYellow);
			SetAimColor(Color.yellow,
					Mathf.Lerp(0f, 1.1f, 1 - aimAngle / startAimAngle), null);

		if (aimAngle <= shootAngle)
		{
			// Shoot!
			HideAim();
			shootAction();
		}
	}

	public void SetAimColor(Color color, float alpha, Material material)
	{
		aimColor = color;
		aimColor.a = alpha;
		aimLeftSpriteRenderer.color = aimColor;
		aimRightSpriteRenderer.color = aimColor;
		aimLeftSpriteRenderer.material = material;
		aimRightSpriteRenderer.material = material;
	}

	public void SetAimAngle(float aimAngle)
	{
		this.aimAngle = aimAngle;
		aimLeftTransform.localEulerAngles = new Vector3(0, 0, +aimAngle);
		aimRightTransform.localEulerAngles = new Vector3(0, 0, -aimAngle);

		//int layerMask = ~(1 << GameAssets.i.enemyLayer | 1 << GameAssets.i.ignoreRaycastLayer | 1 << GameAssets.i.playerLayer | 1 << GameAssets.i.shieldLayer);
		var layerMask = ~0;
		RaycastHit2D raycastHit2D;

		raycastHit2D = Physics2D.Raycast(aimLeftTransform.position,
				UtilsClass.GetVectorFromAngle(aimLeftTransform.eulerAngles.z),
				float.MaxValue, layerMask);
		if (raycastHit2D.collider == null)
			aimLeftTransform.localScale = new Vector3(500, 1, 1);
		else
			aimLeftTransform.localScale
					= new Vector3(raycastHit2D.distance, 1, 1);

		raycastHit2D = Physics2D.Raycast(aimRightTransform.position,
				UtilsClass.GetVectorFromAngle(aimRightTransform.eulerAngles.z),
				float.MaxValue, layerMask);
		if (raycastHit2D.collider == null)
			aimRightTransform.localScale = new Vector3(500, 1, 1);
		else
			aimRightTransform.localScale
					= new Vector3(raycastHit2D.distance, 1, 1);
	}

	public void AimAtPosition(Vector3 aimPosition)
	{
		var aimDir = (aimPosition - GetPosition()).normalized;
		aimTransform.eulerAngles
				= new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(aimDir));
	}

	public bool IsInsideDodgeAngle()
	{
		var dodgeAngle = 4.5f;
		return aimAngle < dodgeAngle;
	}

	public void HideAim()
	{
		aimTransform.gameObject.SetActive(false);
	}

	public void ShowAim()
	{
		aimTransform.gameObject.SetActive(true);
	}

	private Vector3 GetPosition()
	{
		return transform.position;
	}
}
