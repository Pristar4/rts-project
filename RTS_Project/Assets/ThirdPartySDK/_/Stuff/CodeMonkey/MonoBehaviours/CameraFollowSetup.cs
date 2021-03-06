#region Info
// -----------------------------------------------------------------------
// CameraFollowSetup.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion

namespace CodeMonkey.MonoBehaviours
{

	/*
	 * Easy set up for CameraFollow, it will follow the transform with zoom
	 * */
	public class CameraFollowSetup : MonoBehaviour
	{

		[SerializeField] private CameraFollow cameraFollow;
		[SerializeField] private Transform followTransform;
		[SerializeField] private float zoom;

		private void Start()
		{
			if (followTransform == null)
			{
				Debug.LogError("followTransform is null! Intended?");
				cameraFollow.Setup(() => Vector3.zero, () => zoom, true, true);
			}
			else
			{
				cameraFollow.Setup(() => followTransform.position, () => zoom,
						true,
						true);
			}
		}
	}

}
