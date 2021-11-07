#region Info
// -----------------------------------------------------------------------
// CameraFollow.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

namespace CodeMonkey.MonoBehaviours
{

	/*
	 * Script to handle Camera Movement and Zoom
	 * Place on Camera GameObject
	 * */
	public class CameraFollow : MonoBehaviour
	{
		private Func<Vector3> GetCameraFollowPositionFunc;
		private Func<float> GetCameraZoomFunc;

		private Camera myCamera;

		public static CameraFollow Instance { get; private set; }

		private void Awake()
		{
			Instance = this;
			myCamera = transform.GetComponent<Camera>();
		}


		private void Update()
		{
			HandleMovement();
			HandleZoom();
		}

		public void Setup(Func<Vector3> GetCameraFollowPositionFunc,
		                  Func<float> GetCameraZoomFunc,
		                  bool teleportToFollowPosition, bool instantZoom)
		{
			this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
			this.GetCameraZoomFunc = GetCameraZoomFunc;

			if (teleportToFollowPosition)
			{
				var cameraFollowPosition = GetCameraFollowPositionFunc();
				cameraFollowPosition.z = transform.position.z;
				transform.position = cameraFollowPosition;
			}

			if (instantZoom) myCamera.orthographicSize = GetCameraZoomFunc();
		}

		public void SetCameraFollowPosition(Vector3 cameraFollowPosition)
		{
			SetGetCameraFollowPositionFunc(() => cameraFollowPosition);
		}

		public void SetGetCameraFollowPositionFunc(
				Func<Vector3> GetCameraFollowPositionFunc)
		{
			this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
		}

		public void SetCameraZoom(float cameraZoom)
		{
			SetGetCameraZoomFunc(() => cameraZoom);
		}

		public void SetGetCameraZoomFunc(Func<float> GetCameraZoomFunc)
		{
			this.GetCameraZoomFunc = GetCameraZoomFunc;
		}

		private void HandleMovement()
		{
			if (GetCameraFollowPositionFunc == null) return;
			var cameraFollowPosition = GetCameraFollowPositionFunc();
			cameraFollowPosition.z = transform.position.z;

			var cameraMoveDir
					= (cameraFollowPosition - transform.position).normalized;
			var distance
					= Vector3.Distance(cameraFollowPosition,
							transform.position);
			var cameraMoveSpeed = 3f;

			if (distance > 0)
			{
				var newCameraPosition = transform.position + cameraMoveDir *
						distance *
						cameraMoveSpeed * Time.deltaTime;

				var distanceAfterMoving
						= Vector3.Distance(newCameraPosition,
								cameraFollowPosition);

				if (distanceAfterMoving > distance) // Overshot the target
					newCameraPosition = cameraFollowPosition;

				transform.position = newCameraPosition;
			}
		}

		private void HandleZoom()
		{
			if (GetCameraZoomFunc == null) return;
			var cameraZoom = GetCameraZoomFunc();

			var cameraZoomDifference = cameraZoom - myCamera.orthographicSize;
			var cameraZoomSpeed = 1f;

			myCamera.orthographicSize
					+= cameraZoomDifference * cameraZoomSpeed * Time.deltaTime;

			if (cameraZoomDifference > 0)
			{
				if (myCamera.orthographicSize > cameraZoom)
					myCamera.orthographicSize = cameraZoom;
			}
			else
			{
				if (myCamera.orthographicSize < cameraZoom)
					myCamera.orthographicSize = cameraZoom;
			}
		}
	}

}
