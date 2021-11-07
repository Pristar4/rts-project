#region Info
// -----------------------------------------------------------------------
// MoveRoam.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class MoveRoam : MonoBehaviour
{

	private Vector3 startPosition;
	private Vector3 targetMovePosition;

	private void Awake()
	{
		startPosition = transform.position;
	}

	private void Start()
	{
		SetRandomMovePosition();
	}

	private void Update()
	{
		SetMovePosition(targetMovePosition);

		var arrivedAtPositionDistance = 1f;
		if (Vector3.Distance(transform.position, targetMovePosition) <
		    arrivedAtPositionDistance) // Reached position
			SetRandomMovePosition();
	}

	private void SetRandomMovePosition()
	{
		targetMovePosition = startPosition +
		                     UtilsClass.GetRandomDir() *
		                     Random.Range(30f, 100f);
	}

	private void SetMovePosition(Vector3 movePosition)
	{
		GetComponent<IMovePosition>().SetMovePosition(movePosition);
	}
}
