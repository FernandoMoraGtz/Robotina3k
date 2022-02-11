using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeDragNDrop : MonoBehaviour
{
    public GameObject tasksGameObject;
    public ChargeStation chargeStationPreFab;
    private ChargeStation chargeStation;

    private Vector3 originalTransform;
    private Vector3 mOffset;
    private float mZCoord;

    void Start()
    {
        originalTransform = transform.position;
    }
    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    void OnMouseUp()
    {
        Vector3 positionToSpawn = GetMouseAsWorldPoint() + mOffset;
        positionToSpawn.y = 1;
        chargeStation = Instantiate(chargeStationPreFab, GetMouseAsWorldPoint() + mOffset, Quaternion.identity) as ChargeStation;
        chargeStation.transform.parent = tasksGameObject.transform;
        chargeStation.transform.position = positionToSpawn;
        transform.position = originalTransform;
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + mOffset;
    }
}
