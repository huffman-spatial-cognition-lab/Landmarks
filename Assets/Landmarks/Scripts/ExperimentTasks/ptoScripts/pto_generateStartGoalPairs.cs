/*
    pto_generateStartGoalPairs
       
    Generates and populates starting and target locations/objects.

    Sinan Yumurtaci -- Colby College
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pto_generateStartGoalPairs : ExperimentTask
{
    [Header("Task-specific Properties")]
    public GameObject[] stayAwayObjects;
    public float stayAwayDistance;

    public Transform targetLocationParent;
    public Transform startLocationParent;
    public Transform targetObjectParent;
    
    public GameObject targetLocationTemplate;
    public GameObject startLocationTemplate;
    public GameObject targetObjectTemplate;

    public int numberOfTrials;

    private ObjectList startOutputList;
    private ObjectList goalOutputList;

    public override void startTask()
    {
        TASK_START();

        Vector3[] startLocations = new Vector3[3];
        startLocations[0] = new Vector3(-1.5f, 0.5f, 2.5f);
        startLocations[1] = new Vector3(0    , 0.5f, 2.5f);
        startLocations[2] = new Vector3(1.5f , 0.5f, 2.5f);

        Vector3[] targetLocations = new Vector3[3];
        targetLocations[0] = new Vector3(-1.5f, 0.5f, -2.5f);
        targetLocations[1] = new Vector3(0    , 0.5f, -2.5f);
        targetLocations[2] = new Vector3(1.5f , 0.5f, -2.5f);

        GameObject go;

        foreach (Vector3 startLocation in startLocations){
            go = Instantiate(startLocationTemplate, startLocation, Quaternion.identity);
            go.transform.parent = startLocationParent;
        }

        foreach (Vector3 targetLocation in targetLocations){
            go = Instantiate(targetLocationTemplate, targetLocation, Quaternion.identity);
            go.transform.parent = targetLocationParent;
            go = Instantiate(targetObjectTemplate, targetLocation, Quaternion.identity);
            go.transform.parent = targetObjectParent;
        }



    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // WRITE TASK STARTUP CODE HERE
    }


    public override bool updateTask()
    {
        return true;

        // WRITE TASK UPDATE CODE HERE
    }


    public override void endTask()
    {
        TASK_END();

        // LEAVE BLANK
    }


    public override void TASK_END()
    {
        base.endTask();

        // WRITE TASK EXIT CODE HERE
    }

}