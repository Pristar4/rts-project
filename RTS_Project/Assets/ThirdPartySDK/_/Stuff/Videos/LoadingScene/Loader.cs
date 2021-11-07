#region Info
// -----------------------------------------------------------------------
// Loader.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion
public static class Loader
{

	public enum Scene
	{
		GameScene,
		Loading,
		MainMenu
	}

	private static Action onLoaderCallback;
	private static AsyncOperation loadingAsyncOperation;

	public static void Load(Scene scene)
	{
		// Set the loader callback action to load the target scene
		onLoaderCallback = () =>
		{
			var loadingGameObject = new GameObject("Loading Game Object");
			loadingGameObject.AddComponent<LoadingMonoBehaviour>()
					.StartCoroutine(LoadSceneAsync(scene));
		};

		// Load the loading scene
		SceneManager.LoadScene(Scene.Loading.ToString());
	}

	private static IEnumerator LoadSceneAsync(Scene scene)
	{
		yield return null;

		loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

		while (!loadingAsyncOperation.isDone) yield return null;
	}

	public static float GetLoadingProgress()
	{
		if (loadingAsyncOperation != null)
			return loadingAsyncOperation.progress;
		return 1f;
	}

	public static void LoaderCallback()
	{
		// Triggered after the first Update which lets the screen refresh
		// Execute the loader callback action which will load the target scene
		if (onLoaderCallback != null)
		{
			onLoaderCallback();
			onLoaderCallback = null;
		}
	}

	private class LoadingMonoBehaviour : MonoBehaviour
	{
	}
}
