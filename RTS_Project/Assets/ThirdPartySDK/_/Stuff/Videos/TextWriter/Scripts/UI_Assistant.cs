#region Info
// -----------------------------------------------------------------------
// UI_Assistant.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
#endregion
public class UI_Assistant : MonoBehaviour
{

	private Text messageText;
	private AudioSource talkingAudioSource;
	private TextWriter.TextWriterSingle textWriterSingle;

	private void Awake()
	{
		messageText = transform.Find("message").Find("messageText")
				.GetComponent<Text>();
		talkingAudioSource
				= transform.Find("talkingSound").GetComponent<AudioSource>();

		transform.Find("message").GetComponent<Button_UI>().ClickFunc = () =>
		{
			if (textWriterSingle != null && textWriterSingle.IsActive())
			{
				// Currently active TextWriter
				textWriterSingle.WriteAllAndDestroy();
			}
			else
			{
				string[] messageArray =
				{
					"This is the assistant speaking, hello and goodbye, see you next time!",
					"Hey there!",
					"This is a really cool and useful effect",
					"Let's learn some code and make awesome games!",
					"Check out Battle Royale Tycoon on Steam!"
				};

				var message
						= messageArray[Random.Range(0, messageArray.Length)];
				StartTalkingSound();
				textWriterSingle = TextWriter.AddWriter_Static(messageText,
						message,
						.02f, true, true, StopTalkingSound);
			}
		};
	}

	private void Start()
	{
		//TextWriter.AddWriter_Static(messageText, "This is the assistant speaking, hello and goodbye, see you next time!", .1f, true);
	}

	private void StartTalkingSound()
	{
		talkingAudioSource.Play();
	}

	private void StopTalkingSound()
	{
		talkingAudioSource.Stop();
	}
}
