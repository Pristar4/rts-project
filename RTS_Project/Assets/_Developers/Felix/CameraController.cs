#region Info
// -----------------------------------------------------------------------
// CameraController.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using Cinemachine;
using UnityEngine;
#endregion
public class CameraController : MonoBehaviour
{
	[SerializeField] private float panSpeed = 100f;
	[SerializeField] private float zoomSpeed = 30f;
	[SerializeField] private float dragSpeed = 1f;

	[SerializeField] private float zoomInMax = 35f;
	[SerializeField] private float zoomOutMax = 120f;
	private Transform cameraTransform;


	private bool drag;


	private CinemachineInputProvider inputProvider;
	private Vector3 oldPos;
	private Vector3 panOrigin;
	private CinemachineVirtualCamera virtualCamera;

	private void Awake()
	{
		inputProvider = GetComponent<CinemachineInputProvider>();
		virtualCamera = GetComponent<CinemachineVirtualCamera>();
		cameraTransform = virtualCamera.VirtualCameraGameObject.transform;
	}

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Confined;
	}

	private void Update()
	{
		var x = inputProvider.GetAxisValue(0);
		var y = inputProvider.GetAxisValue(1);
		var z = inputProvider.GetAxisValue(2);
		if (x != 0 || y != 0)
			if (!drag)
				PanScreen(x, y);

		if (z != 0)
			if (!drag)
				ZoomScreen(z);

		UpdateDrag();
		if (drag)
			Cursor.visible = false;
		else
			Cursor.visible = true;
	}

	private void UpdateDrag()
	{
		if (Input.GetMouseButtonDown(2))
		{
			drag = true;
			oldPos = transform.position;
			panOrigin
					= Camera.main.ScreenToViewportPoint(Input
							.mousePosition); //Get the ScreenVector the mouse clicked
		}

		if (Input.GetMouseButton(2))
		{
			var pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) -
			          panOrigin; //Get the difference between where the mouse clicked and where it moved
			transform.position =
					oldPos + -pos *
					dragSpeed; //Move the position of the camera to simulate a drag, speed * 10 for screen to worldspace conversion
		}

		if (Input.GetMouseButtonUp(2)) drag = false;
	}

	public void ZoomScreen(float increment)
	{
		var fov = virtualCamera.m_Lens.OrthographicSize;
		var target = Mathf.Clamp(fov + increment, zoomInMax, zoomOutMax);
		virtualCamera.m_Lens.OrthographicSize
				= Mathf.Lerp(fov, target, zoomSpeed * Time.deltaTime);
	}

	public Vector2 PanDirection(float x, float y)
	{
		var direction = Vector2.zero;
		if (y >= Screen.height * 0.95f)
			direction.y += 1;
		else if (y <= Screen.height * 0.05f)
			direction.y -= 1;
		else if (x >= Screen.width * 0.95f)
			direction.x += 1;
		else if (x <= Screen.width * 0.05f) direction.x -= 1;

		return direction;
	}


	public void PanScreen(float x, float y)
	{
		var direction = PanDirection(x, y);
		cameraTransform.position = Vector3.Lerp(cameraTransform.position,
				cameraTransform.position + (Vector3)direction * panSpeed,
				Time.deltaTime);
	}
}
