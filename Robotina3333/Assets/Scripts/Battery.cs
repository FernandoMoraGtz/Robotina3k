using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public int charge = 30;
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            return hit.transform.gameObject;
        else
            return null;
    }
    void OnCollisionEnter(Collision collision) //Propiocepción
    {
        robotina.addEnergyToRobotina(charge);
        robotina.goingToBattery = false;
        Destroy(gameObject);
    }
}
