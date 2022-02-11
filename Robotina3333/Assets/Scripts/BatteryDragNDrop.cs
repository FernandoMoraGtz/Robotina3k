using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryDragNDrop : MonoBehaviour
{
    public GameObject tasksGameObject;
    public Battery batteryTaskPreFab;
    private Battery batteryTask;

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
        batteryTask = Instantiate(batteryTaskPreFab, GetMouseAsWorldPoint() + mOffset, Quaternion.identity) as Battery;
        batteryTask.transform.parent = tasksGameObject.transform;
        batteryTask.transform.position = positionToSpawn;
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
