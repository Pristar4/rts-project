#region Info
// -----------------------------------------------------------------------
// HitCounter.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion
public class HitCounter : MonoBehaviour
{
	private Vector3 baseLocalPosition;
	private int hitCount;

	//[SerializeField] private Player player;
	private MeshRenderer meshRenderer;
	private float shakeIntensity;
	private TextMeshPro textMeshPro;

	private void Awake()
	{
		textMeshPro = GetComponent<TextMeshPro>();
		meshRenderer = GetComponent<MeshRenderer>();
		baseLocalPosition = transform.localPosition;

		HideHitCounter();
	}

	private void Start()
	{
		/*
		player.OnAttacked += Player_OnAttacked;
		player.OnDamaged += Player_OnDamaged;
		player.OnEnemyHit += Player_OnEnemyHit;
		player.OnEnemyKilled += Player_OnEnemyKilled;
		*/
	}

	private void Update()
	{
		if (shakeIntensity > 0)
		{
			var shakeIntensityDropAmount = .5f;
			shakeIntensity -= shakeIntensityDropAmount * Time.deltaTime;
			var randomDirection
					= new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f))
							.normalized;
			transform.localPosition
					= baseLocalPosition + randomDirection * shakeIntensity;
		}
	}

	private void Player_OnEnemyKilled(object sender, EventArgs e)
	{
		// Player killed an Enemy
		IncreaseHitCount();

		UtilsClass.ShakeCamera(3f, .1f);
	}

	private void Player_OnEnemyHit(object sender, EventArgs e)
	{
		// Player hit an Enemy
		IncreaseHitCount();

		var baseIntensity = .2f;
		var perHitIntensity = .02f;
		shakeIntensity = Mathf.Clamp(baseIntensity + perHitIntensity * hitCount,
				baseIntensity, 1.2f);

		UtilsClass.ShakeCamera(.3f, .1f);
	}

	private void Player_OnDamaged(object sender, EventArgs e)
	{
		// Player took Damage
		hitCount = 0;
		HideHitCounter();
	}

	private void Player_OnAttacked(object sender, EventArgs e)
	{
		// Player did an Attack
		IncreaseHitCount();
	}

	private void IncreaseHitCount()
	{
		hitCount++;
		SetHitCounter(hitCount);
	}

	private void SetHitCounter(int hitCount)
	{
		textMeshPro.SetText(hitCount.ToString());
		meshRenderer.enabled = true;

		var textColor = Color.white;

		if (hitCount >= 10) textColor = UtilsClass.GetColorFromString("00A0FF");
		if (hitCount >= 20) textColor = UtilsClass.GetColorFromString("24E100");
		if (hitCount >= 30) textColor = UtilsClass.GetColorFromString("FFE300");
		if (hitCount >= 40) textColor = UtilsClass.GetColorFromString("FF7F1C");
		if (hitCount >= 50) textColor = UtilsClass.GetColorFromString("FF3AF2");

		textMeshPro.color = textColor;

		var startingFontSize = 6f;
		var perHitFontSize = .06f;
		textMeshPro.fontSize
				= Mathf.Clamp(startingFontSize + perHitFontSize * hitCount,
						startingFontSize, startingFontSize * 2f);
	}

	private void HideHitCounter()
	{
		meshRenderer.enabled = false;
	}
}
