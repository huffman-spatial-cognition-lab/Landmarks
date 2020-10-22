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

        List<List<float>> trialMatrix = new List<List<float>>();

        List<float> distanceList = new List<float> {3.5f, 4.5f};
        List<float> borderList =new List<float>  {0f, 1f};

        numberOfTrials = trialRepeatCount * distanceList.Count * borderList.Count;

        int count = 0;
        foreach (int distance in distanceList){
            foreach (int border in borderList){
                for(int i = 0; i < trialRepeatCount; i++){
                    trialMatrix.Add(new List<float> {count, distance,border});
                    count++;
                }
            }  
        }

        Vector3[] startLocations = new Vector3[numberOfTrials];
        Vector3[] targetLocations = new Vector3[numberOfTrials];

        Random.seed = 42;

        // get the x and z range for our generation from the ground object
        
        Vector3 lastTargetLocation = new Vector3(0, 0, 0);
        for (int i = 0; i < numberOfTrials; i++){

            List<Vector3> generatedLocations = generateStartGoalPair(trialMatrix[i][1], lastTargetLocation, true, ground.transform);

            startLocations[i] = generatedLocations[0];
            targetLocations[i] = generatedLocations[1];
            lastTargetLocation = targetLocations[i];
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

    private List<Vector3> generateStartGoalPair(float distance, Vector3 lastLocation, bool passBorder, Transform groundT){
        
        Vector3 startLocation;
        Vector3 targetLocation;
        Vector2 deltaVector; 
        float betweenTrialDistance;
        bool isPairValid = true;

        float xMin = groundT.position.x - groundT.lossyScale.x / 2 + minimumWallDistance;
        float xMax = groundT.position.x + groundT.lossyScale.x / 2 - minimumWallDistance;
        float zMin = groundT.position.y - groundT.lossyScale.z / 2 + minimumWallDistance;
        float zMax = groundT.position.y + groundT.lossyScale.z / 2 - minimumWallDistance;
        float borderZ = (zMin + zMax) / 2;

        do{

            // set up random starting location and direction
            startLocation = new Vector3(Random.Range(xMin, xMax), 0.5f, Random.Range(zMin, zMax));
            deltaVector = Random.insideUnitCircle.normalized * distance;

            targetLocation = new Vector3(
                startLocation.x + deltaVector.x,
                0.5f, 
                startLocation.z + deltaVector.y
            );
            
            betweenTrialDistance = Vector3.Distance(lastLocation,startLocation);
            

            isPairValid = true; // assume valid until proven otherwise

            if (targetLocation.x < xMin | targetLocation.x > xMax){
                isPairValid = false;
            }
            if (targetLocation.z < zMin | targetLocation.z > zMax){
                isPairValid = false;
            }
            if (betweenTrialDistance < minimumBetweenTrialDistance){
                isPairValid = false;
            }
            if (passBorder){
                if( (startLocation.z < borderZ) == (targetLocation.z < borderZ) ){ // if on the same side of the border x-boundary
                    isPairValid = false;
                }
            }

        }while(!isPairValid);

        return new List<Vector3> {startLocation, targetLocation};
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