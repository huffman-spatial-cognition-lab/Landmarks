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

    public int trialRepeatCount;
    public float minimumWallDistance;
    public float minimumTravelDistance;
    public float minimumBetweenTrialDistance = 2;

    public Transform targetLocationParent;
    public Transform startLocationParent;
    public Transform targetObjectParent;
    
    public GameObject targetLocationTemplate;
    public GameObject startLocationTemplate;
    public GameObject targetObjectTemplate;

    private int numberOfTrials;

    public GameObject ground;

    private ObjectList startOutputList;
    private ObjectList goalOutputList;

    

    public override void startTask()
    {
        TASK_START();

        

        List<List<double>> trialMatrix = new List<List<double>>();

        List<double> distanceList = new List<double> {2, 2.5, 3, 3.5};
        List<double> borderList =new List<double>  {0, 1};

        numberOfTrials = trialRepeatCount * distanceList.Count * borderList.Count;

        int count = 0;
        foreach (int distance in distanceList){
            foreach (int border in borderList){
                for(int i = 0; i < trialRepeatCount; i++){
                    trialMatrix.Add(new List<double> {count, distance,border});
                    count++;
                }
            }  
        }

        Vector3[] startLocations = new Vector3[numberOfTrials];
        Vector3[] targetLocations = new Vector3[numberOfTrials];

        Random.seed = 42;

        // get the x and z range for our generation from the ground object
        Transform groundT = ground.transform;
        float xMin = groundT.position.x - groundT.lossyScale.x / 2 + minimumWallDistance;
        float xMax = groundT.position.x + groundT.lossyScale.x / 2 - minimumWallDistance;
        float zMin = groundT.position.y - groundT.lossyScale.z / 2 + minimumWallDistance;
        float zMax = groundT.position.y + groundT.lossyScale.z / 2 - minimumWallDistance;

        for (int i = 0; i < numberOfTrials; i++){
            Vector3 startLocation;
            Vector3 targetLocation;
            float travelDistance;
            float betweenTrialDistance;
            bool isPairValid = true;
            do{

                // set up random locations
                startLocation = new Vector3(Random.Range(xMin, xMax), 0.5f, Random.Range(zMin, zMax));
                targetLocation = new Vector3(Random.Range(xMin, xMax), 0.5f, Random.Range(zMin, zMax));
                
                // test conditions
                
                travelDistance = Vector3.Distance(startLocation, targetLocation);

                if (i==0){
                    betweenTrialDistance = Vector3.Distance(new Vector3(0,0,0), startLocation);
                }else{
                    betweenTrialDistance = Vector3.Distance(targetLocations[i-1],startLocation);
                }

                isPairValid = true; // assume valid until proven otherwise

                if (travelDistance < minimumTravelDistance){
                    isPairValid = false;
                }

                if (betweenTrialDistance < minimumBetweenTrialDistance){
                    isPairValid = false;
                }

            }while(!isPairValid);

            Debug.Log(betweenTrialDistance);

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