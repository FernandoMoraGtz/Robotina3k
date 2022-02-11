using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsDragNDrop : MonoBehaviour
{
    public GameObject cubePreFab;

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
        GameObject cube = Instantiate(cubePreFab, GetMouseAsWorldPoint() + mOffset, Quaternion.identity) as GameObject;
        cube.transform.position = positionToSpawn;
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
