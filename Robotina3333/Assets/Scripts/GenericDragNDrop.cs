using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericDragNDrop : MonoBehaviour
{
    public GameObject tasksGameObjectParent;
    public GenericTask genericTaskPreFab;
    public int taskCost;
    private GenericTask genericTask;

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
        genericTask = Instantiate(genericTaskPreFab, GetMouseAsWorldPoint() + mOffset, Quaternion.identity) as GenericTask;
        genericTask.transform.parent = tasksGameObjectParent.transform;
        genericTask.transform.position = positionToSpawn;
        genericTask.taskCost = taskCost;
        genericTask.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;
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
