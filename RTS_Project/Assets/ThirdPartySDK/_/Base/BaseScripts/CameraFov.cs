#region Info
// -----------------------------------------------------------------------
// CameraFov.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion
public class CameraFov : MonoBehaviour
{
	private float fov;

	private Camera playerCamera;
	private float targetFov;

	private void Awake()
	{
		playerCamera = GetComponent<Camera>();
		targetFov = playerCamera.fieldOfView;
		fov = targetFov;
	}

	private void Update()
	{
		var fovSpeed = 4f;
		fov = Mathf.Lerp(fov, targetFov, Time.deltaTime * fovSpeed);
		playerCamera.fieldOfView = fov;
	}

	public void SetCameraFov(float targetFov)
	{
		this.targetFov = targetFov;
	}
}
