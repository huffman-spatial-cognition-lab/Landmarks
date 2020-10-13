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
    public float stayAwayObjectsDistance;
    public float minimumTravelDistance;

    public Transform targetLocationParent;
    public Transform startLocationParent;
    public Transform targetObjectParent;
    
    public GameObject targetLocationTemplate;
    public GameObject startLocationTemplate;
    public GameObject targetObjectTemplate;

    public int numberOfTrials;

    public GameObject ground;

    private ObjectList startOutputList;
    private ObjectList goalOutputList;

    public override void startTask()
    {
        TASK_START();


        Vector3[] startLocations = new Vector3[numberOfTrials];
        Vector3[] targetLocations = new Vector3[numberOfTrials];

        Random.seed = 42;

        // get the x and z range for our generation from the ground object
        Transform groundT = ground.transform;
        float xMin = groundT.position.x - groundT.lossyScale.x / 2;
        float xMax = groundT.position.x + groundT.lossyScale.x / 2;
        float zMin = groundT.position.y - groundT.lossyScale.z / 2;
        float zMax = groundT.position.y + groundT.lossyScale.z / 2;

        Debug.Log(xMin);
        Debug.Log(xMax);Debug.Log(zMin);Debug.Log(zMax);

        for (int i = 0; i < numberOfTrials; i++){
            Vector3 startLocation;
            Vector3 targetLocation;
            do{

                // set up random locations
                startLocation = new Vector3(Random.Range(xMin, xMax), 0.5f, Random.Range(zMin, zMax));
                targetLocation = new Vector3(Random.Range(xMin, xMax), 0.5f, Random.Range(zMin, zMax));
                
                // test conditions

                // test: distance to each other

                // test: distance to stayAwayObjects

            }while(false);

            startLocations[i] = startLocation;
            targetLocations[i] = targetLocation;
        }

        /*
        for debugging/demonstration: a simple, 3-trial based hard-coded experiment set-up
        startLocations[0] = new Vector3(-1.5f, 0.5f, 2.5f);
        startLocations[1] = new Vector3(0    , 0.5f, 2.5f);
        startLocations[2] = new Vector3(1.5f , 0.5f, 2.5f);
        targetLocations[0] = new Vector3(-1.5f, 0.5f, -2.5f);
        targetLocations[1] = new Vector3(0    , 0.5f, -2.5f);
        targetLocations[2] = new Vector3(1.5f , 0.5f, -2.5f);
        */
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