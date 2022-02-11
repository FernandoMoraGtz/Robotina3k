using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Robotina : MonoBehaviour
{
    //Base Energy
    public int maxEnergy = 100;
    public int currentEnergy;

    //Energy Bar
    public EnergyBar energyBar;
    public TextPercent textPercent;
    public TextPercent textPercentMap;

    //Translation
    public bool goingToChargeStation = false;
    public bool goingToBattery = false;

    //Tasks
    private GenericTask[] tasks;

    //Power Sources
    private Battery[] batteryTasks;
    private ChargeStation[] chargeStations;

    //Steering Vars
    private float speed = 4f;
    private float step;
    public int numberOfRays = 17;
    public float angle = 90;
    public float rayRange = 1;

    //Idle Drain Speed
    public float drainSpeed;

    void Start()
    {
        currentEnergy = maxEnergy;
        energyBar.setMaxEnergy(maxEnergy);
        step = speed * Time.deltaTime;
        InvokeRepeating("substractPassiveEnergy", 1.0f, drainSpeed);
    }

    void FixedUpdate() //Atención
    {
        findObjectsInField();
        planRoute();
    }
    void findObjectsInField() //Percepción
    {
        //Memoria
        tasks = FindObjectsOfType<GenericTask>();
        batteryTasks = FindObjectsOfType<Battery>();
        chargeStations = FindObjectsOfType<ChargeStation>();
    }

    private void planRoute() //Planeación
    {
        Transform taskToMoveInto = null;
        ChargeStation chargeStation = findClosestChargeStation();
        //Checks if full charge is Needed
        if (chargeStation != null && currentEnergy <= 15 && !goingToBattery)
        {
            goingToChargeStation = true;
            taskToMoveInto = chargeStation.transform; //Toma de decisión
        }
        //Checks if a task is doable
        GenericTask genericTask = getBestPossibleTask(); //Planeación
        if (genericTask != null && !goingToChargeStation)
        {
            Battery closestBattery = findClosestBattery(); // Planeación
            Transform genericTaskTransform = genericTask.transform;

            var costUntilTask = genericTask.taskCost + getBatteryPercentDrainUntilObjectCollision(transform, genericTaskTransform);
            var costUntilChargeStation = costUntilTask + getBatteryPercentDrainUntilObjectCollision(genericTaskTransform, chargeStation.transform);
            var isBatteryCloserThanTask = closestBattery != null 
                && getDistanceToObjectFromRobotina(closestBattery.transform) < getDistanceToObjectFromRobotina(genericTaskTransform.transform);

            if (currentEnergy < 60 && isBatteryCloserThanTask && currentEnergy + closestBattery.charge > costUntilChargeStation)
            {
                goingToBattery = true;
                taskToMoveInto = closestBattery.transform; //Toma de decisión
            }
            else if(currentEnergy - costUntilChargeStation > 5)
                taskToMoveInto = genericTaskTransform; //Toma de decisión
        }
        //Moves into best possible task
        if (taskToMoveInto != null)
            moveToTask(taskToMoveInto); //Toma de decisión
    }

    private GenericTask getBestPossibleTask() //Planeación
    {
        int indexOfClosestTask = 0;
        List<float> taskDistanceList = new List<float>();

        if (tasks.Length == 1)
            indexOfClosestTask = 0; //Toma de decisión
        else if (tasks.Length > 1)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                taskDistanceList.Add(getDistanceToObjectFromRobotina(tasks[i].transform));
            }

            while (taskDistanceList.Count > 0)
            {
                indexOfClosestTask = taskDistanceList.IndexOf(taskDistanceList.Min());
                if (isGivenTaskDoable(tasks[indexOfClosestTask]))
                {
                    return tasks[indexOfClosestTask]; //Toma de decisión
                }
                else
                {
                    taskDistanceList.RemoveAt(indexOfClosestTask);
                }
            }
        }
        else
            return null;

        return tasks[indexOfClosestTask]; //Toma de decisión
    }

    private bool isGivenTaskDoable(GenericTask genericTask) //Planeación
    {
        Battery closestBattery = findClosestBattery();
        ChargeStation chargeStation = findClosestChargeStation();
        Transform genericTaskTransform = genericTask.transform;
        

        var costUntilTask = genericTask.taskCost + getBatteryPercentDrainUntilObjectCollision(transform, genericTaskTransform);
        var costUntilChargeStation = costUntilTask + getBatteryPercentDrainUntilObjectCollision(genericTaskTransform, chargeStation.transform);
        var isBatteryCloserThanTask = closestBattery != null && getDistanceToObjectFromRobotina(closestBattery.transform) < getDistanceToObjectFromRobotina(genericTaskTransform.transform);

        if (currentEnergy - costUntilChargeStation > 1 ||
            (isBatteryCloserThanTask && currentEnergy + closestBattery.charge > costUntilChargeStation))
        {
            return true; //Toma de decisión
        }

        return false; //Toma de decisión
    }

    private Battery findClosestBattery() // Planeación
    {
        int indexOfClosestBattery = 0;
        if (batteryTasks.Length == 1)
            indexOfClosestBattery = 0; //Toma de decisión
        else if (batteryTasks.Length == 0)
            return null; //Toma de decisión
        else
            for (int i = 0; i < batteryTasks.Length - 1; i++)
                if (getDistanceToObjectFromRobotina(batteryTasks[i].transform) < getDistanceToObjectFromRobotina(batteryTasks[i + 1].transform))
                    indexOfClosestBattery = i; //Toma de decisión
                else
                    indexOfClosestBattery = i + 1; //Toma de decisión
        return batteryTasks[indexOfClosestBattery]; //Toma de decisión
    }

    private ChargeStation findClosestChargeStation() // Planeación
    {
        int indexOfClosestChargeStation = 0;
        if (chargeStations.Length == 1)
            indexOfClosestChargeStation = 0; //Toma de decisión
        else if (chargeStations.Length == 0)
            return null; //Toma de decisión
        else
            for (int i = 0; i < batteryTasks.Length - 1; i++)
                if (getDistanceToObjectFromRobotina(chargeStations[i].transform) < getDistanceToObjectFromRobotina(chargeStations[i + 1].transform))
                    indexOfClosestChargeStation = i; //Toma de decisión
                else
                    indexOfClosestChargeStation = i + 1; //Toma de decisión
        return chargeStations[indexOfClosestChargeStation]; //Toma de decisión
    }

    private void moveToTask(Transform taskTransform) // Movimiento Motor
    {
        if (taskTransform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, taskTransform.transform.position, step); // Movimiento
        }
    }
    private float getDistanceToObjectFromRobotina(Transform taskTransform) // Planeación
    {
        return Vector3.Distance(taskTransform.position, transform.position);
    }

    private float getBatteryPercentDrainUntilObjectCollision(Transform origin, Transform destination) // Planeación
    {
        return Vector3.Distance(origin.position, destination.position) / speed;
    }

    #region Battery Management Propiocepción
    private void substractPassiveEnergy()
    {
            currentEnergy-=5;
            updateEnergy(currentEnergy);
    }

    public void substractEnergyFromTask(int taskEnergy)
    {
        currentEnergy -= taskEnergy;
        updateEnergy(currentEnergy);
    }
    public void addEnergyToRobotina(int charge)
    {
        currentEnergy += charge;
        currentEnergy = currentEnergy > 100 ? maxEnergy : currentEnergy;
        updateEnergy(currentEnergy);
    }
    private void updateEnergy(int currentEnergy)
    {
        if (currentEnergy <= 0)
            currentEnergy = 0;
        energyBar.setEnergy(currentEnergy);
        textPercent.updateText(currentEnergy);
        textPercentMap.updateMapText(currentEnergy);
    }
    #endregion
    void OnDrawGizmos()
    {
        for (int i = 0; i < numberOfRays; ++i)
        {
            var rotation = this.transform.rotation;
            var rotationMod = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle * 2 - angle, this.transform.up);
            var direction = rotation * rotationMod * Vector3.forward;
            Gizmos.DrawRay(this.transform.position, direction);
        }
    }
}
