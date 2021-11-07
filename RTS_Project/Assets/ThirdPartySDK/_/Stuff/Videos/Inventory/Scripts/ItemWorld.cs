#region Info
// -----------------------------------------------------------------------
// ItemWorld.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
#endregion
public class ItemWorld : MonoBehaviour
{


	private Item item;
	private Light2D light2D;
	private SpriteRenderer spriteRenderer;
	private TextMeshPro textMeshPro;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		light2D = transform.Find("Light").GetComponent<Light2D>();
		textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
	}

	public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
	{
		var transform = Instantiate(ItemAssets.Instance.pfItemWorld, position,
				Quaternion.identity);

		var itemWorld = transform.GetComponent<ItemWorld>();
		itemWorld.SetItem(item);

		return itemWorld;
	}

	public static ItemWorld DropItem(Vector3 dropPosition, Item item)
	{
		var randomDir = UtilsClass.GetRandomDir();
		var itemWorld = SpawnItemWorld(dropPosition + randomDir * 8f, item);
		itemWorld.GetComponent<Rigidbody2D>()
				.AddForce(randomDir * 40f, ForceMode2D.Impulse);
		return itemWorld;
	}

	public void SetItem(Item item)
	{
		this.item = item;
		spriteRenderer.sprite = item.GetSprite();
		light2D.color = item.GetColor();
		if (item.amount > 1)
			textMeshPro.SetText(item.amount.ToString());
		else
			textMeshPro.SetText("");
	}

	public Item GetItem()
	{
		return item;
	}

	public void DestroySelf()
	{
		Destroy(gameObject);
	}
}
