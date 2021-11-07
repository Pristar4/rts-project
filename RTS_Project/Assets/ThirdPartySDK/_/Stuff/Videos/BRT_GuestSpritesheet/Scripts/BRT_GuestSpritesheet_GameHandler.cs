#region Info
// -----------------------------------------------------------------------
// BRT_GuestSpritesheet_GameHandler.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey;
using UnityEngine;
#endregion
public class BRT_GuestSpritesheet_GameHandler : MonoBehaviour
{

	[SerializeField] private Texture2D baseTexture;
	[SerializeField] private Texture2D headTexture;
	[SerializeField] private Texture2D bodyTexture;
	[SerializeField] private Texture2D bodyTextureWhite;
	[SerializeField] private Texture2D bodyTextureMask;
	[SerializeField] private Texture2D hairTexture;
	[SerializeField] private Texture2D beardTexture;
	[SerializeField] private Texture2D baseHeadTexture;
	[SerializeField] private Texture2D baseHeadMaskTexture;
	[SerializeField] private Texture2D handTexture;
	[SerializeField] private Material guestMaterial;
	private GuestSpritesheetData guestSpritesheetData;

	private Color primaryColor = Color.red;
	private Color secondaryColor = Color.yellow;

	private void Awake()
	{
		guestSpritesheetData = GuestSpritesheetData.GenerateRandom();
		SetGuestSpritesheetData(guestSpritesheetData);

		CMDebug.ButtonUI(new Vector2(100, -170), "Randomize", () =>
		{
			guestSpritesheetData = GuestSpritesheetData.GenerateRandom();
			SetGuestSpritesheetData(guestSpritesheetData);
		});
		CMDebug.ButtonUI(new Vector2(250, -170), "Save", () =>
		{
			guestSpritesheetData.Save();
			CMDebug.TextPopupMouse("Saved!");
		});
		CMDebug.ButtonUI(new Vector2(400, -170), "Load", () =>
		{
			guestSpritesheetData = GuestSpritesheetData.Load_Static();
			SetGuestSpritesheetData(guestSpritesheetData);
			CMDebug.TextPopupMouse("Loaded!");
		});
	}

	private void SetGuestSpritesheetData(
			GuestSpritesheetData guestSpritesheetData)
	{
		var texture = GetTexture(guestSpritesheetData);
		guestMaterial.mainTexture = texture;
	}

	private Texture2D GetTexture(GuestSpritesheetData guestSpritesheetData)
	{
		var texture = new Texture2D(512, 512, TextureFormat.RGBA32, true);

		var spritesheetBasePixels = baseTexture.GetPixels(0, 0, 512, 512);
		texture.SetPixels(0, 0, 512, 512, spritesheetBasePixels);

		var skinColor = guestSpritesheetData.skinColor;

		var headPixels = baseHeadTexture.GetPixels(0, 0, 128, 128);
		var headSkinMaskPixels = baseHeadMaskTexture.GetPixels(0, 0, 128, 128);
		TintColorArraysInsideMask(headPixels, skinColor, headSkinMaskPixels);

		var handPixels = handTexture.GetPixels(0, 0, 64, 64);
		TintColorArray(handPixels, skinColor);
		texture.SetPixels(384, 448, 64, 64, handPixels);

		var hairColor = guestSpritesheetData.hairColor;

		var hasHair = guestSpritesheetData.hairIndex != -1;
		if (hasHair)
		{
			var hairIndex = guestSpritesheetData.hairIndex;
			var hairPixels
					= hairTexture.GetPixels(128 * hairIndex, 0, 128, 128);
			TintColorArray(hairPixels, hairColor);
			MergeColorArrays(headPixels, hairPixels);
		}

		var hasBeard = guestSpritesheetData.beardIndex != -1;
		if (hasBeard)
		{
			var beardIndex = guestSpritesheetData.beardIndex;
			var beardPixels
					= beardTexture.GetPixels(128 * beardIndex, 0, 128, 128);
			TintColorArray(beardPixels, hairColor);
			MergeColorArrays(headPixels, beardPixels);
		}

		texture.SetPixels(0, 384, 128, 128, headPixels);

		var bodyIndex = guestSpritesheetData.bodyIndex;
		var bodyPixels
				= bodyTextureWhite.GetPixels(128 * bodyIndex, 0, 128, 128);
		var bodyMaskPixels
				= bodyTextureMask.GetPixels(128 * bodyIndex, 0, 128, 128);
		var primaryColor = guestSpritesheetData.bodyPrimaryColor;
		var primaryMaskColor = new Color(0, 1, 0);
		TintColorArraysInsideMask(bodyPixels, primaryColor, bodyMaskPixels,
				primaryMaskColor);
		var secondaryColor = guestSpritesheetData.bodySecondaryColor;
		var secondaryMaskColor = new Color(0, 0, 1);
		TintColorArraysInsideMask(bodyPixels, secondaryColor, bodyMaskPixels,
				secondaryMaskColor);
		texture.SetPixels(0, 256, 128, 128, bodyPixels);

		texture.Apply();

		return texture;
	}

	private void MergeColorArrays(Color[] baseArray, Color[] overlay)
	{
		for (var i = 0; i < baseArray.Length; i++)
			if (overlay[i].a > 0)
			{
				// Overlay has color
				if (overlay[i].a >= 1)
				{
					// Fully replace
					baseArray[i] = overlay[i];
				}
				else
				{
					// Interpolate colors
					var alpha = overlay[i].a;
					baseArray[i].r += (overlay[i].r - baseArray[i].r) * alpha;
					baseArray[i].g += (overlay[i].g - baseArray[i].g) * alpha;
					baseArray[i].b += (overlay[i].b - baseArray[i].b) * alpha;
					baseArray[i].a += overlay[i].a;
				}
			}
	}

	private void TintColorArray(Color[] baseArray, Color tint)
	{
		for (var i = 0; i < baseArray.Length; i++)
		{
			// Apply tint
			baseArray[i].r = baseArray[i].r * tint.r;
			baseArray[i].g = baseArray[i].g * tint.g;
			baseArray[i].b = baseArray[i].b * tint.b;
		}
	}

	private void TintColorArraysInsideMask(Color[] baseArray, Color tint,
	                                       Color[] mask)
	{
		for (var i = 0; i < baseArray.Length; i++)
			if (mask[i].a > 0)
			{
				// Apply tint
				var baseColor = baseArray[i];
				var fullyTintedColor = tint * baseColor;
				var interpolateAmount = mask[i].a;
				baseArray[i].r
						+= (fullyTintedColor.r - baseColor.r) *
						   interpolateAmount;
				baseArray[i].g
						+= (fullyTintedColor.g - baseColor.g) *
						   interpolateAmount;
				baseArray[i].b
						+= (fullyTintedColor.b - baseColor.b) *
						   interpolateAmount;
			}

	}

	private void TintColorArraysInsideMask(Color[] baseArray, Color tint,
	                                       Color[] mask, Color maskColor)
	{
		for (var i = 0; i < baseArray.Length; i++)
			if (mask[i].a > 0 && mask[i] == maskColor)
			{
				// Apply tint
				var baseColor = baseArray[i];
				var fullyTintedColor = tint * baseColor;
				var interpolateAmount = mask[i].a;
				baseArray[i].r
						+= (fullyTintedColor.r - baseColor.r) *
						   interpolateAmount;
				baseArray[i].g
						+= (fullyTintedColor.g - baseColor.g) *
						   interpolateAmount;
				baseArray[i].b
						+= (fullyTintedColor.b - baseColor.b) *
						   interpolateAmount;
			}

	}
}
