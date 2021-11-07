#region Info
// -----------------------------------------------------------------------
// LoaderCallback.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class LoaderCallback : MonoBehaviour
{

	private bool isFirstUpdate = true;

	private void Update()
	{
		if (isFirstUpdate)
		{
			isFirstUpdate = false;
			Loader.LoaderCallback();
		}
	}
}
