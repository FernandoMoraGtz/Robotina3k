using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTask : MonoBehaviour
{
    [SerializeField]
    public int taskCost;

    public Robotina robotina;

    public LayerMask layerMask;


    void Start()
    {
        robotina = (Robotina)FindObjectOfType(typeof(Robotina));
    }
    void Update()
    {
        GameObject clickedGmObj;

        if (Input.GetMouseButtonDown(1))
        {
            clickedGmObj = GetClickedGameObject();
            if (clickedGmObj != null)
            {
                Destroy(clickedGmObj);
            }
        }
    }
    GameObject GetClickedGameObject()
    {
        // Builds a ray from camera point of view to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Casts the ray and get the first game object hit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            return hit.transform.gameObject;
        else
            return null;
    }
    void OnCollisionEnter(Collision collision) //Propiocepción 
    {
        robotina.substractEnergyFromTask(taskCost);
        Destroy(gameObject);
    }
}
