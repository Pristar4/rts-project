using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.InputSystem;

public class GameRTSController : MonoBehaviour
{
    [SerializeField] private Transform selectionAreaTransform;

    private Vector3 startPosition;
    private List<UnitRTS> selectedUnitRTSList;
    [SerializeField] private InputActionReference actionReference;
    private InputAction test;

    private bool doubleclick = false;


    private void Awake()
    {
        selectedUnitRTSList = new List<UnitRTS>();
        selectionAreaTransform.gameObject.SetActive(false);
    }

    private void Start()
    {
        actionReference.action.performed += context => { };
    }

    private void Update()
    {
        #region DoubleClickSelect

        if (actionReference.action.triggered)
        {
            doubleclick = true;

            // Left Mouse Button Doubble Press


            //Corner locations in world coordinates
            var upperRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            var lowerLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));


            selectionAreaTransform.position = lowerLeft;
            selectionAreaTransform.localPosition = upperRight - lowerLeft;
            Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(lowerLeft, upperRight);
            //Deselect all Units
            foreach (UnitRTS unitRTS in selectedUnitRTSList)
            {
                unitRTS.SetSelectedVisible(false);
            }

            selectedUnitRTSList.Clear();
            // Select All Units on Screen
            foreach (Collider2D collider2D in collider2DArray)
            {
                UnitRTS unitRTS = collider2D.GetComponent<UnitRTS>();
                if (unitRTS != null)
                {
                    unitRTS.SetSelectedVisible(true);
                    selectedUnitRTSList.Add(unitRTS);
                }
            }
        }
        else
        {
            doubleclick = false;
        }

        #endregion

        if (!doubleclick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Left Mouse Button Pressed
                selectionAreaTransform.gameObject.SetActive(true);
                startPosition = UtilsClass.GetMouseWorldPosition();
            }

            if (Input.GetMouseButton(0))
            {
                // Left Mouse Button Held Down
                Vector3 currentMousePosition = UtilsClass.GetMouseWorldPosition();
                Vector3 lowerLeft = new Vector3(
                    Mathf.Min(startPosition.x, currentMousePosition.x),
                    Mathf.Min(startPosition.y, currentMousePosition.y)
                );
                Vector3 upperRight = new Vector3(
                    Mathf.Max(startPosition.x, currentMousePosition.x),
                    Mathf.Max(startPosition.y, currentMousePosition.y)
                );
                selectionAreaTransform.position = lowerLeft;
                selectionAreaTransform.localScale = upperRight - lowerLeft;
            }

            if (Input.GetMouseButtonUp(0))
            {
                // Left Mouse Button Released
                selectionAreaTransform.gameObject.SetActive(false);

                Collider2D[] collider2DArray =
                    Physics2D.OverlapAreaAll(startPosition, UtilsClass.GetMouseWorldPosition());

                // Deselect all Units
                foreach (UnitRTS unitRTS in selectedUnitRTSList)
                {
                    unitRTS.SetSelectedVisible(false);
                }

                selectedUnitRTSList.Clear();

                // Select Units within Selection Area
                foreach (Collider2D collider2D in collider2DArray)
                {
                    UnitRTS unitRTS = collider2D.GetComponent<UnitRTS>();
                    if (unitRTS != null)
                    {
                        unitRTS.SetSelectedVisible(true);
                        selectedUnitRTSList.Add(unitRTS);
                    }
                }
            }

            //Debug.Log(selectedUnitRTSList.Count);
        }

        if (Input.GetMouseButtonDown(1))
        {
            // Right Mouse Button Pressed
            Vector3 moveToPosition = UtilsClass.GetMouseWorldPosition();

            List<Vector3> targetPositionList =
                GetPositionListAround(moveToPosition, new float[] { 10f, 20f, 30f }, new int[] { 5, 10, 20 });

            int targetPositionListIndex = 0;

            foreach (UnitRTS unitRTS in selectedUnitRTSList)
            {
                unitRTS.MoveTo(targetPositionList[targetPositionListIndex]);
                targetPositionListIndex = (targetPositionListIndex + 1) % targetPositionList.Count;
            }
        }
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistanceArray,
        int[] ringPositionCountArray)
    {
        List<Vector3> positionList = new List<Vector3>();
        positionList.Add(startPosition);
        for (int i = 0; i < ringDistanceArray.Length; i++)
        {
            positionList.AddRange(GetPositionListAround(startPosition, ringDistanceArray[i],
                ringPositionCountArray[i]));
        }

        return positionList;
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * (90 / positionCount);
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            Vector3 position = startPosition + dir * distance;
            positionList.Add(position);
        }


        return positionList;
    }

    private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }


    /*#region OnEnable+Disable

    private void OnEnable()
    {
        actionReference.action.Enable();
    }

    private void OnDisable()
    {
        actionReference.action.Disable();
    }

    #endregion*/
}
