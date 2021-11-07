#region Info
// -----------------------------------------------------------------------
// HeartsHealthVisual.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class HeartsHealthVisual : MonoBehaviour
{

	public static HeartsHealthSystem heartsHealthSystemStatic;

	[SerializeField] private Sprite heart0Sprite;
	[SerializeField] private Sprite heart1Sprite;
	[SerializeField] private Sprite heart2Sprite;
	[SerializeField] private Sprite heart3Sprite;
	[SerializeField] private Sprite heart4Sprite;
	[SerializeField] private AnimationClip heartFullAnimationClip;

	private List<HeartImage> heartImageList;
	private HeartsHealthSystem heartsHealthSystem;
	private bool isHealing;

	private void Awake()
	{
		heartImageList = new List<HeartImage>();
	}

	private void Start()
	{
		FunctionPeriodic.Create(HealingAnimatedPeriodic, .05f);
		var heartsHealthSystem = new HeartsHealthSystem(20);
		SetHeartsHealthSystem(heartsHealthSystem);

		/*
		//Testing buttons
		CMDebug.ButtonUI(new Vector2(-50, -100), "Damage 1", () => heartsHealthSystem.Damage(1));
		CMDebug.ButtonUI(new Vector2(50, -100), "Damage 4", () => heartsHealthSystem.Damage(4));
		
		CMDebug.ButtonUI(new Vector2(-50, -200), "Heal 1", () => heartsHealthSystem.Heal(1));
		CMDebug.ButtonUI(new Vector2(50, -200), "Heal 4", () => heartsHealthSystem.Heal(4));
		CMDebug.ButtonUI(new Vector2(150, -200), "Heal 50", () => heartsHealthSystem.Heal(50));
		*/
	}

	public void SetHeartsHealthSystem(HeartsHealthSystem heartsHealthSystem)
	{
		this.heartsHealthSystem = heartsHealthSystem;
		heartsHealthSystemStatic = heartsHealthSystem;

		var heartList = heartsHealthSystem.GetHeartList();
		var row = 0;
		var col = 0;
		var colMax = 10;
		var rowColSize = 30f;

		for (var i = 0; i < heartList.Count; i++)
		{
			var heart = heartList[i];
			var heartAnchoredPosition
					= new Vector2(col * rowColSize, -row * rowColSize);
			CreateHeartImage(heartAnchoredPosition)
					.SetHeartFraments(heart.GetFragmentAmount());

			col++;
			if (col >= colMax)
			{
				row++;
				col = 0;
			}
		}

		heartsHealthSystem.OnDamaged += HeartsHealthSystem_OnDamaged;
		heartsHealthSystem.OnHealed += HeartsHealthSystem_OnHealed;
		heartsHealthSystem.OnDead += HeartsHealthSystem_OnDead;
	}

	private void HeartsHealthSystem_OnDead(object sender, EventArgs e)
	{
		CMDebug.TextPopupMouse("Dead!");
	}

	private void HeartsHealthSystem_OnHealed(object sender, EventArgs e)
	{
		// Hearts health system was healed
		//RefreshAllHearts();
		isHealing = true;
	}

	private void HeartsHealthSystem_OnDamaged(object sender, EventArgs e)
	{
		// Hearts health system was damaged
		RefreshAllHearts();
	}

	private void RefreshAllHearts()
	{
		var heartList = heartsHealthSystem.GetHeartList();
		for (var i = 0; i < heartImageList.Count; i++)
		{
			var heartImage = heartImageList[i];
			var heart = heartList[i];
			heartImage.SetHeartFraments(heart.GetFragmentAmount());
		}
	}

	private void HealingAnimatedPeriodic()
	{
		if (isHealing)
		{
			var fullyHealed = true;
			var heartList = heartsHealthSystem.GetHeartList();
			for (var i = 0; i < heartList.Count; i++)
			{
				var heartImage = heartImageList[i];
				var heart = heartList[i];
				if (heartImage.GetFragmentAmount() != heart.GetFragmentAmount())
				{
					// Visual is different from logic
					heartImage.AddHeartVisualFragment();
					if (heartImage.GetFragmentAmount() ==
					    HeartsHealthSystem
							    .MAX_FRAGMENT_AMOUNT) // This heart was fully healed
						heartImage.PlayHeartFullAnimation();
					fullyHealed = false;
					break;
				}
			}
			if (fullyHealed) isHealing = false;
		}
	}

	private HeartImage CreateHeartImage(Vector2 anchoredPosition)
	{
		// Create Game Object
		var heartGameObject
				= new GameObject("Heart", typeof(Image), typeof(Animation));

		// Set as child of this transform
		heartGameObject.transform.parent = transform;
		heartGameObject.transform.localPosition = Vector3.zero;
		heartGameObject.transform.localScale = Vector3.one;

		// Locate and Size heart
		heartGameObject.GetComponent<RectTransform>().anchoredPosition
				= anchoredPosition;
		heartGameObject.GetComponent<RectTransform>().sizeDelta
				= new Vector2(35, 35);

		heartGameObject.GetComponent<Animation>()
				.AddClip(heartFullAnimationClip, "HeartFull");

		// Set heart sprite
		var heartImageUI = heartGameObject.GetComponent<Image>();
		heartImageUI.sprite = heart4Sprite;

		var heartImage = new HeartImage(this, heartImageUI,
				heartGameObject.GetComponent<Animation>());
		heartImageList.Add(heartImage);

		return heartImage;
	}


	// Represents a single Heart
	public class HeartImage
	{
		private readonly Animation animation;
		private readonly Image heartImage;
		private readonly HeartsHealthVisual heartsHealthVisual;

		private int fragments;

		public HeartImage(HeartsHealthVisual heartsHealthVisual,
		                  Image heartImage,
		                  Animation animation)
		{
			this.heartsHealthVisual = heartsHealthVisual;
			this.heartImage = heartImage;
			this.animation = animation;
		}

		public void SetHeartFraments(int fragments)
		{
			this.fragments = fragments;
			switch (fragments)
			{
				case 0:
					heartImage.sprite = heartsHealthVisual.heart0Sprite;
					break;
				case 1:
					heartImage.sprite = heartsHealthVisual.heart1Sprite;
					break;
				case 2:
					heartImage.sprite = heartsHealthVisual.heart2Sprite;
					break;
				case 3:
					heartImage.sprite = heartsHealthVisual.heart3Sprite;
					break;
				case 4:
					heartImage.sprite = heartsHealthVisual.heart4Sprite;
					break;
			}
		}

		public int GetFragmentAmount()
		{
			return fragments;
		}

		public void AddHeartVisualFragment()
		{
			SetHeartFraments(fragments + 1);
		}

		public void PlayHeartFullAnimation()
		{
			animation.Play("HeartFull", PlayMode.StopAll);
		}
	}
}
