﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

enum PointingTaskStage{
    Orienting,
    Pointing
}

public class LM_ObjectPlacement : ExperimentTask
{

    private Trial trialData;
    private int numObjects;
    private int currObjInd;
	private GameObject currentPlacementObject;

    [Header("Task-specific Properties")]
    public GameObject infiniteTerrain;
    public GameObject relocationTargetTemplateParent;
    public GameObject pointingObjectParent;
    public GameObject markerObjectTemplate;
    public LineRenderer lineRendererTemplate;
    public GameObject handModelLeft;
    public GameObject handModelRight;
    [Range(0.0f, 100.0f)]
    public float markerStartDistance;
    [Range(-1.0f, 3.0f)]
    public float markerFixedHeight;
    public float translationSpeed;

    public Transform RightHand;

    public bool responseCooldown = true;
    public int responseCooldownMs = 500;

    // Private Variables
    
    private GameObject markerObject;
    private Vector3 markerLocation;
    private PointingTaskStage stage = PointingTaskStage.Orienting;
    private bool triggerWasPushed = false;

    private LineRenderer _lineRenderer;

    private long timeStarted;

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // WRITE TASK STARTUP CODE HERE

        // load data from our ground-truth
        // repeatCount starts from 1, we subtract one to 0-index into the arrays
        trialData = GameObject.Find("TrialsTruth").GetComponent<pto_trialsTruth>().trialsTruth.trials[this.parentTask.repeatCount - 1];
        numObjects = trialData.targetObjects.Count - 1; // minus 1 for the last object, which is for the pointing task.
        currObjInd = 0;
        
        lineRendererTemplate.gameObject.SetActive(false);
		_lineRenderer = Instantiate(lineRendererTemplate);

        hud.showEverything();

        // removing the HUD by setting display time to 0 seconds
        hud.setMessage("");
        hud.SecondsToShow = 0;

    }

    public override bool updateTask()
    {
        if (vrEnabled){
            OVRInput.Update(); // required to track the Oculus input
        }

        bool vrInput = false;

        // calculate vrInput (whether the trigger is being pressed is used as the input in this experiment)
        if(!triggerWasPushed){
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) >= 0.75f){ // right index finger on the trigger pushed beyond .75
                triggerWasPushed = true;
            } 
        }else{
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) <= 0.5f){ // right index finger on the trigger pushed less than .5
                triggerWasPushed = false;
                vrInput = true; // true for only one frame
            } 
        }

        // During the orienting stage
        if (stage == PointingTaskStage.Orienting){
            
            Debug.Log("vrEnabled? " + vrEnabled);
            Debug.Log("Return key pressed? " + Input.GetKeyDown(KeyCode.Return));
            if ((Input.GetKeyDown(KeyCode.Return)) || (vrEnabled && vrInput))
            {
                
                if (vrEnabled)
                {
                    // VR-specific actions should go here
                    // Currently none.
                }
                else 
                {
                    // Lock player movement & reset looking at the current orientation ONLY IF KEYBOARD CONTROLS
                    avatar.GetComponent<FirstPersonController>().enabled = false; // disable the controller to work
                }

                // log the information about the player's current location (note: other bits will be in the previous line, e.g., rotation)
                log.log("OBJECT_PLACEMENT\tPLAYER_ORIENTED\tLOCATION:\t" + manager.player.transform.position.x + "\t" + manager.player.transform.position.y + "\t" + manager.player.transform.position.z, 1);

                // Instantiate the Marker
                markerLocation = manager.player.transform.position + avatar.GetComponentInChildren<Camera>().transform.forward * markerStartDistance;

                markerObject = Instantiate(markerObjectTemplate, markerLocation, Quaternion.identity, pointingObjectParent.transform);
                initializeObjectForPlacement();

                // and log the start of the new trial
                string taskStringForLog = gatherTaskStringForLog("OBJECT_INSTANTIATED", new Vector3(), new Vector3());
                log.log(taskStringForLog, 1);
                
                // move on to the next stage
                stage = PointingTaskStage.Pointing;
                Debug.Log("Orienting stage complete, moving to placing markers (ie Pointing)");

                // get the current time as the start time for the first pointing object
                timeStarted = Experiment.Now();
            }
        }

        // During the pointing stage (after orienting)
        else if (stage == PointingTaskStage.Pointing){
            
            Vector3 startPoint = new Vector3();
            Vector3 endPoint = new Vector3();

            if (vrEnabled)
            {
                
                int range = 100;
                bool aimHit = false;
                Collider terrainCollider = infiniteTerrain.GetComponent<BoxCollider>();
		        Ray aimRay = new Ray(RightHand.position, RightHand.forward);
                startPoint = RightHand.position;
                endPoint = startPoint + aimRay.direction * range;
                RaycastHit hitInfo;

                if(terrainCollider.Raycast(aimRay, out hitInfo, range)){
                    endPoint = startPoint + aimRay.direction * hitInfo.distance;
                    aimHit = true;
                }

                Vector3 moveMarkersTo = new Vector3(hitInfo.point.x, 1.0f, hitInfo.point.z);

                markerObject.transform.position = moveMarkersTo;
                currentPlacementObject.transform.position = moveMarkersTo;

                _lineRenderer.gameObject.SetActive(true);
                _lineRenderer.positionCount = 2;
                _lineRenderer.SetPosition(0, startPoint);
                _lineRenderer.SetPosition(1, endPoint);
               
            }
            else
            {
                int targetMovementHori = 0;
                int targetMovementVert = 0;
                Vector3 targetTranslation = Vector3.zero;
                if(Input.GetKey(KeyCode.U)){
                    targetMovementVert = 1;
                }
                if (Input.GetKey(KeyCode.M)){
                    targetMovementVert = -1;
                }
                if (Input.GetKey(KeyCode.H)){
                    targetMovementHori = -1;
                }
                if (Input.GetKey(KeyCode.K)){
                    targetMovementHori = 1;  
                }
                
                if (targetMovementHori != 0){
                    targetTranslation += Vector3.right * Time.deltaTime * targetMovementHori;
                }
                if (targetMovementVert != 0){
                    targetTranslation += Vector3.forward * Time.deltaTime * targetMovementVert;
                }
                Debug.Log(targetTranslation);
                markerLocation = markerObject.transform.position;
                markerLocation += targetTranslation * translationSpeed;
                markerLocation.y = markerFixedHeight;

                markerObject.transform.position = markerLocation;
                currentPlacementObject.transform.position = markerLocation;
            }


            // submit the position
            if ((Input.GetKeyDown(KeyCode.Return)) || (vrEnabled && vrInput)){
                // if we do not want to use a response cooldown OR the responseCooldownMs has passed
                if (!responseCooldown || Experiment.Now() >= (timeStarted + responseCooldownMs))
                {
                    //log.log("OBJECT_PLACEMENT\tOBJECT_PLACED\tRAY_START:\t" + startPoint.x + "\t" + startPoint.y + "\t"+ startPoint.z + "\t"+
                    //    "RAY_END:\t" + endPoint.x + "\t" + endPoint.y + "\t"+ endPoint.z, 1);
                    string taskStringForLog = gatherTaskStringForLog("OBJECT_PLACED", startPoint, endPoint);
                    log.log(taskStringForLog, 1);

                    currentPlacementObject.SetActive(false);
                    currentPlacementObject = null;
                    currObjInd++;

                    Debug.Log(currObjInd);
                    Debug.Log(numObjects);
                    if (currObjInd == numObjects)
                    { // are we done with all the objects? if so, move on from this task
                        Debug.Log("completed!");
                        return true;
                    }

                    initializeObjectForPlacement();

                    // and log the start of the new trial
                    taskStringForLog = gatherTaskStringForLog("OBJECT_INSTANTIATED", startPoint, endPoint);
                    log.log(taskStringForLog, 1);

                    // and update the timeStarted for the next object
                    timeStarted = Experiment.Now();
                } else
                {
                    // if the response cooldown has not passed, then do not move on and log this info
                    string taskStringForLog = gatherTaskStringForLog("PRESSED_RETURN_BEFORE_COOLDOWN", startPoint, endPoint);
                    log.log(taskStringForLog, 1);
                }
            } else
            {
                // log the raycast and object information (e.g., so we can recreate the object "path" from log)
                string taskUpdateStringForLog = gatherTaskStringForLog("OBJECT_MOVING", startPoint, endPoint);
                log.log(taskUpdateStringForLog, 1);
            }
        }

        return false;
        
    }


    public override void endTask()
    {
        TASK_END();

        // LEAVE BLANK
    }


    public override void TASK_END()
    {
        base.endTask();

        // --------------------------
        // Log data
        // --------------------------

        log.log("LM_ObjectPlacement.cs\tTASK_END");
            /*
            trialLog.AddData(transform.name + "_playerLocation", startAngle.ToString()); // record where we started the compass at
            trialLog.AddData(transform.name + "_responseCW", response.ToString());
            trialLog.AddData(transform.name + "_answerCW", answer.ToString());
            trialLog.AddData(transform.name + "_signedError", signedError.ToString());
            trialLog.AddData(transform.name + "_absError", absError.ToString());
            trialLog.AddData(transform.name + "_SOPorientingTime", orientTime.ToString());
            trialLog.AddData(transform.name + "_responseTime", responseTime.ToString());
            */

        // Object clean-up
        Destroy(markerObject);
        Destroy(_lineRenderer);

        if(!vrEnabled){
            avatar.GetComponent<FirstPersonController>().enabled = true;
        }

        stage = PointingTaskStage.Orienting; // reset stage to the original state.
    }

    private void initializeObjectForPlacement(){
        string targetObjectString = trialData.targetObjects[currObjInd].object_repr;
        GameObject targetObject = relocationTargetTemplateParent.transform.Find(targetObjectString).gameObject;
        GameObject go = Instantiate(targetObject, pointingObjectParent.transform);
        currentPlacementObject = go;
        go.transform.position = new Vector3(0f, 1.0f, 0f);
    }

    private string gatherTaskStringForLog(string beginOrEndingString, Vector3 startPoint, Vector3 endPoint)
    {
        // set up strings for the outpu, here this is the prefix
        string stringForLog = "LM_ObjectPlacement.cs\t" + beginOrEndingString + "\t";
        // add the information about the object index and number of objects
        stringForLog += "object_ind\t" + currObjInd.ToString() + "\tnum_objects\t" + numObjects.ToString();
        // add the information about the object name
        stringForLog += "\tobject_name\t" + currentPlacementObject.transform.gameObject.name;
        // add the information about the scale
        //stringForLog += "\tscale_x\t" + scale_x.ToString() + "\tscale_y\t" + scale_y.ToString();
        // add the information about the x and y coordinates of the object
        //stringForLog += "\tfinal_x\t" + final_x.ToString() + "\tfinal_y\t" + final_y.ToString();
        // add the information about the start point
        stringForLog += "\tRAY_START:\t" + startPoint.x + "\t" + startPoint.y + "\t" + startPoint.z;
        // add the information about the end point
        stringForLog += "\tRAY_END:\t" + endPoint.x + "\t" + endPoint.y + "\t" + endPoint.z;
        // add the information about the object position
        stringForLog += "\tOBJECT_POS_X\t" + currentPlacementObject.transform.position.x;
        stringForLog += "\tOBJECT_POS_Y\t" + currentPlacementObject.transform.position.y;
        stringForLog += "\tOBJECT_POS_Z\t" + currentPlacementObject.transform.position.z;
        return stringForLog;
    }

}