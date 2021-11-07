#region Info
// -----------------------------------------------------------------------
// GameHandler_Setup.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.MonoBehaviours;
using CodeMonkey.Utils;
using GridPathfindingSystem;
using UnityEngine;
#endregion
public class GameHandler_Setup : MonoBehaviour
{
	public static GridPathfinding gridPathfinding;

	[SerializeField] private CameraFollow cameraFollow;
	[SerializeField] private float cameraZoom;
	[SerializeField] private Transform followTransform;
	[SerializeField] private bool cameraPositionWithMouse;

	[SerializeField] private CharacterAimHandler characterAimHandler;
	[SerializeField] private CharacterAim_Base characterAimBase;

	private void Start()
	{
		//Sound_Manager.Init();
		cameraFollow.Setup(GetCameraPosition,
				() => cameraZoom == -1 ? 60f : cameraZoom,
				true, true);

		//FunctionPeriodic.Create(SpawnEnemy, 1.5f);
		//for (int i = 0; i < 1000; i++) SpawnEnemy();

		gridPathfinding = new GridPathfinding(new Vector3(-400, -400),
				new Vector3(400, 400), 5f);
		gridPathfinding.RaycastWalkable();

		//EnemyHandler.Create(new Vector3(20, 0));

		if (characterAimHandler != null)
			characterAimHandler.OnShoot += CharacterAimHandler_OnShoot;

		if (characterAimBase != null)
			characterAimBase.OnShoot += CharacterAimBase_OnShoot;
	}

	private void CharacterAimBase_OnShoot(object sender,
	                                      CharacterAim_Base.OnShootEventArgs e)
	{
		Shoot_Flash.AddFlash(e.gunEndPointPosition);
		WeaponTracer.Create(e.gunEndPointPosition, e.shootPosition);
		UtilsClass.ShakeCamera(.6f, .05f);

		// Any enemy hit?
		var raycastHit = Physics2D.Raycast(e.gunEndPointPosition,
				(e.shootPosition - e.gunEndPointPosition).normalized,
				Vector3.Distance(e.gunEndPointPosition, e.shootPosition));
		if (raycastHit.collider != null)
		{
			var enemyHandler
					= raycastHit.collider.gameObject
							.GetComponent<EnemyHandler>();
			if (enemyHandler != null)
				Debug.Log("Cannot Damage!");
			//enemyHandler.Damage(characterAimBase);
		}
	}

	private void CharacterAimHandler_OnShoot(object sender,
	                                         CharacterAimHandler.
			                                         OnShootEventArgs
			                                         e)
	{
		Shoot_Flash.AddFlash(e.gunEndPointPosition);
		WeaponTracer.Create(e.gunEndPointPosition, e.shootPosition);
		UtilsClass.ShakeCamera(.6f, .05f);

		// Any enemy hit?
		var raycastHit = Physics2D.Raycast(e.gunEndPointPosition,
				(e.shootPosition - e.gunEndPointPosition).normalized,
				Vector3.Distance(e.gunEndPointPosition, e.shootPosition));
		if (raycastHit.collider != null)
		{
			var enemyHandler
					= raycastHit.collider.gameObject
							.GetComponent<EnemyHandler>();
			if (enemyHandler != null) enemyHandler.Damage(characterAimHandler);
		}
	}

	private Vector3 GetCameraPosition()
	{
		if (followTransform == null) return cameraFollow.transform.position;

		if (cameraPositionWithMouse)
		{
			var mousePosition = UtilsClass.GetMouseWorldPosition();
			var playerToMouseDirection
					= mousePosition - followTransform.position;
			return followTransform.position + playerToMouseDirection * .3f;
		}
		return followTransform.position;
	}

	private void SpawnEnemy()
	{
		var spawnPosition = Vector3.zero + UtilsClass.GetRandomDir() * 40f;
		if (characterAimHandler != null)
			spawnPosition = characterAimHandler.GetPosition() +
			                UtilsClass.GetRandomDir() * 40f;

		EnemyHandler.Create(spawnPosition);
	}
	/*
	private void Update() {
	    if (Input.GetMouseButtonDown(0)) {
	        Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(UtilsClass.GetMouseWorldPosition(), UtilsClass.GetMouseWorldPosition());
	        Debug.Log("########");
	        foreach (Collider2D collider2D in collider2DArray) {
	            Debug.Log(collider2D);
	        }
	    }
	}
	*/
}
