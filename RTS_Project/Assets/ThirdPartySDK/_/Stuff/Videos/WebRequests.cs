#region Info
// -----------------------------------------------------------------------
// WebRequests.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
#endregion
public static class WebRequests
{

	private static WebRequestsMonoBehaviour webRequestsMonoBehaviour;

	private static void Init()
	{
		if (webRequestsMonoBehaviour == null)
		{
			var gameObject = new GameObject("WebRequests");
			webRequestsMonoBehaviour
					= gameObject.AddComponent<WebRequestsMonoBehaviour>();
		}
	}

	public static void Get(string url, Action<string> onError,
	                       Action<string> onSuccess)
	{
		Init();
		webRequestsMonoBehaviour.StartCoroutine(GetCoroutine(url, onError,
				onSuccess));
	}

	private static IEnumerator GetCoroutine(string url, Action<string> onError,
	                                        Action<string> onSuccess)
	{
		using (var unityWebRequest = UnityWebRequest.Get(url))
		{
			yield return unityWebRequest.SendWebRequest();

			if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
				onError(unityWebRequest.error);
			else
				onSuccess(unityWebRequest.downloadHandler.text);
		}
	}

	public static void GetTexture(string url, Action<string> onError,
	                              Action<Texture2D> onSuccess)
	{
		Init();
		webRequestsMonoBehaviour.StartCoroutine(
				GetTextureCoroutine(url, onError, onSuccess));
	}

	private static IEnumerator GetTextureCoroutine(
			string url, Action<string> onError, Action<Texture2D> onSuccess)
	{
		using (var unityWebRequest = UnityWebRequestTexture.GetTexture(url))
		{
			yield return unityWebRequest.SendWebRequest();

			if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
			{
				onError(unityWebRequest.error);
			}
			else
			{
				var downloadHandlerTexture
						= unityWebRequest.downloadHandler as
								DownloadHandlerTexture;
				onSuccess(downloadHandlerTexture.texture);
			}
		}
	}

	private class WebRequestsMonoBehaviour : MonoBehaviour
	{
	}
}
