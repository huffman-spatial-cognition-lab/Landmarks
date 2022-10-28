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

    // Private Variables

    private dbLog log;
	private Experiment manager;
    
    private GameObject markerObject;
    private Vector3 markerLocation;
    private PointingTaskStage stage = PointingTaskStage.Orienting;
    private bool triggerWasPushed = false;

    private LineRenderer _lineRenderer;

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
        trialData = GameObject.Find("TrialsTruth").GetComponent<pto_trialsTruth>().trialsTruth.trials[this.parentTask.repeatCount];
        numObjects = trialData.targetObjects.Count - 1; // minus 1 for the last object, which is for the pointing task.
        currObjInd = 0;
        
        lineRendererTemplate.gameObject.SetActive(false);
		_lineRenderer = Instantiate(lineRendererTemplate);

        manager = experiment.GetComponent("Experiment") as Experiment;
		log = manager.dblog;

        handModelLeft.SetActive(false);
        handModelRight.SetActive(true);

        hud.showEverything();
        
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

        Debug.Log(stage);
        Debug.Log(vrInput);
        Debug.Log(triggerWasPushed);

        // During the orienting stage
        if (stage == PointingTaskStage.Orienting){
            
            if ((!vrEnabled && Input.GetKeyDown(KeyCode.Return)) || (vrEnabled && vrInput))
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

                // Instantiate the Marker
                markerLocation = manager.player.transform.position + avatar.GetComponentInChildren<Camera>().transform.forward * markerStartDistance;
                pointingObjectParent.transform.position = markerLocation;

                markerObject = Instantiate(markerObjectTemplate, new Vector3(0f, 0f, 0f), Quaternion.identity, pointingObjectParent.transform);
                initializeObjectForPlacement();
                //markerObject.transform.localPosition = new Vector3(0,0, -markerStartDistance);    
                //markerObject.transform.localEulerAngles = Vector3.zero;

                log.log("OBJECT_PLACEMENT\tPLAYER_ORIENTED\tLOCATION:\t" + manager.player.transform.position.x + "\t" + manager.player.transform.position.y + "\t"+ manager.player.transform.position.z, 1);
                
                // move on to the next stage
                stage = PointingTaskStage.Pointing;
            }
        }

        // During the pointing stage (after orienting)
        else if (stage == PointingTaskStage.Pointing){

            // check for key updates
            // and move markerLocation accordingly
            // and update the pointingObjectParent.Transform
            Vector3 startPoint = new Vector3();
            Vector3 endPoint = new Vector3();

            if (vrEnabled)
            {
                int range = 100;
                bool aimHit = false;
		        Ray aimRay = new Ray(RightHand.position, RightHand.forward);
                startPoint = RightHand.position;
                endPoint = startPoint + aimRay.direction * range;
                RaycastHit hitInfo;

                Debug.Log(startPoint);

                if(Physics.Raycast(startPoint, aimRay.direction, out hitInfo, range)){//, out hitInfo, range, QueryTriggerInteraction.Ignore)){
                    endPoint = startPoint + aimRay.direction * hitInfo.distance;
                    aimHit = true;
                    Debug.Log(hitInfo);
                    Debug.Log(hitInfo.point);
                    Debug.Log(hitInfo.collider);
                }

                pointingObjectParent.transform.position = hitInfo.point;

                _lineRenderer.gameObject.SetActive(true);
                _lineRenderer.positionCount = 2;
                _lineRenderer.SetPosition(0, startPoint);
                _lineRenderer.SetPosition(1, endPoint);

                // TODO: Put the object at the end of line, a little higher.
               
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
                    targetTranslation += avatar.GetComponentInChildren<Camera>().transform.right * Time.deltaTime * targetMovementHori;
                }
                if (targetMovementVert != 0){
                    targetTranslation += avatar.GetComponentInChildren<Camera>().transform.forward * Time.deltaTime * targetMovementVert;
                    Debug.Log(targetTranslation);
                }
                markerLocation = markerObject.transform.position;
                markerLocation += targetTranslation * translationSpeed;
                markerLocation.y = markerFixedHeight;
                markerObject.transform.position = markerLocation; 
            }

            // submit the position
            if ((!vrEnabled && Input.GetKeyDown(KeyCode.Return)) || (vrEnabled && vrInput)){

                log.log("OBJECT_PLACEMENT\tOBJECT_PLACED\tRAY_START:\t" + startPoint.x + "\t" + startPoint.y + "\t"+ startPoint.z + "\t"+
                    "RAY_END:\t" + endPoint.x + "\t" + endPoint.y + "\t"+ endPoint.z, 1);

                currentPlacementObject.SetActive(false);
                currentPlacementObject = null;
                currObjInd++;

                Debug.Log(currObjInd);
                Debug.Log(numObjects);
                if(currObjInd == numObjects){ // are we done with all the objects? if so, move on from this task
                    Debug.Log("completed!");
                    return true;
                }
                
                initializeObjectForPlacement();

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

        handModelLeft.SetActive(false);
        handModelRight.SetActive(false);

        if(!vrEnabled){
            avatar.GetComponent<FirstPersonController>().enabled = true;
        }

        stage = PointingTaskStage.Orienting; // reset stage to the original state.
    }

    private void initializeObjectForPlacement(){
        string targetObjectString = trialData.targetObjects[currObjInd].object_repr;
        GameObject targetObject = relocationTargetTemplateParent.transform.Find(targetObjectString).gameObject;
        
        GameObject go = Instantiate(targetObject, pointingObjectParent.transform);
        go.transform.position = new Vector3(0f, 1.0f, 0f);
    }

}