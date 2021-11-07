#region Info
// -----------------------------------------------------------------------
// SpriteAnimator.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class SpriteAnimator : MonoBehaviour
{
	public delegate void OnLoopDel();

	public Sprite[] frames;
	public int framesPerSecond = 30;
	public bool loop = true;
	public bool useUnscaledDeltaTime;
	public bool destroyOnLoop;
	private int currentFrame;
	private bool isActive = true;
	public OnLoopDel onLoop;
	private SpriteRenderer spriteRenderer;
	private float timer;
	private float timerMax;

	private void Awake()
	{
		timerMax = 1f / framesPerSecond;
		spriteRenderer = transform.GetComponent<SpriteRenderer>();
		if (frames != null)
			spriteRenderer.sprite = frames[0];
		else
			isActive = false;
	}

	private void Update()
	{
		if (!isActive) return;
		timer += useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
		var newFrame = false;
		while (timer >= timerMax)
		{
			timer -= timerMax;
			//Next frame
			currentFrame = (currentFrame + 1) % frames.Length;
			newFrame = true;
			if (currentFrame == 0)
			{
				//Looped
				if (!loop)
				{
					isActive = false;
					newFrame = false;
				}
				if (onLoop != null) onLoop();
				if (destroyOnLoop)
				{
					Destroy(gameObject);
					return;
				}
			}
		}
		if (newFrame) spriteRenderer.sprite = frames[currentFrame];
	}

	public void Setup(Sprite[] frames, int framesPerSecond)
	{
		this.frames = frames;
		this.framesPerSecond = framesPerSecond;
		timerMax = 1f / framesPerSecond;
		spriteRenderer.sprite = frames[0];
		timer = 0f;
		PlayStart();
	}

	public void PlayStart()
	{
		timer = 0;
		currentFrame = 0;
		spriteRenderer.sprite = frames[currentFrame];
		isActive = true;
	}
}
