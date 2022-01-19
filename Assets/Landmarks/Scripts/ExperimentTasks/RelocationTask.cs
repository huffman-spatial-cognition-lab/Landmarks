using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RelocationTask : ExperimentTask
{
    [Header("Task-specific Properties")]
    private Trial trialData;
    private int numObjects;
    private int currObjInd;
	private GameObject current;
    private bool completedCurrentObject;

    public bool hideNonTargets;

    // For logging output
    private float startTime;
    private Vector3 playerLastPosition;
    private float playerDistance = 0;
    private Vector3 scaledPlayerLastPosition;
    private float scaledPlayerDistance = 0;
    private float optimalDistance;

    // for the alternate method checking if the task is over
    private GameObject CenterEyeAnchor;
    private float alternateDistThreshold = 0.32f;
    private LineRenderer _lineRenderer;
    public LineRenderer lineRendererTemplate;

    // for creating relocation targets
    public GameObject relocationFinalTargetPrefab;
    public GameObject relocationTargetTemplateParent;
    public GameObject currentTargetObjectHolder;

    public override void startTask ()
	{
		TASK_START();
		avatarLog.navLog = true;
        if (isScaled) scaledAvatarLog.navLog = true;
    }

	public override void TASK_START()
	{
		if (!manager) Start();
        base.startTask();

        if (skip)
        {
            log.log("INFO    skip task    " + name, 1);
            return;
        }
        hud.showEverything();

        // load data from our ground-truth
        trialData = GameObject.Find("TrialsTruth").GetComponent<pto_trialsTruth>().trialsTruth.trials[this.parentTask.repeatCount];
        numObjects = trialData.targetObjects.Count;
        currObjInd = 0;
        completedCurrentObject = false;
        relocateTargetStart();


        hud.SecondsToShow = 0;
        hud.setMessage("Please relocate to the target");
        
        try
        {
            current.GetComponent<MeshRenderer>().enabled = true;
        }
        catch (System.Exception ex)
        {
        }

        // startTime = Current time in seconds
        startTime = Time.time;

        // Get the avatar start location (distance = 0)
        playerDistance = 0.0f;
        playerLastPosition = avatar.transform.position;
        if (isScaled)
        {
            scaledPlayerDistance = 0.0f;
            scaledPlayerLastPosition = scaledAvatar.transform.position;
        }

        // Calculate optimal distance to travel (straight line)
        if (isScaled)
        {
            optimalDistance = Vector3.Distance(scaledAvatar.transform.position, current.transform.position);
        }
        else optimalDistance = Vector3.Distance(avatar.transform.position, current.transform.position);

        // store the CenterEyeAnchor so that we do not search for it every game loop (expensive)
        CenterEyeAnchor = GameObject.Find("TrackingSpace/CenterEyeAnchor");
        _lineRenderer = Instantiate(lineRendererTemplate);

    }

    public override bool updateTask ()
	{
		base.updateTask();
        
        if (skip)
        {
            //log.log("INFO    skip task    " + name,1 );
            return true;
        }

        if (completedCurrentObject){
            relocateTargetEnd();
            Debug.Log(currObjInd);
            Debug.Log(numObjects);
            if(currObjInd == numObjects){ // are we done with all the objects? if so, move on from this task
                Debug.Log("completed!");
                return true;
            }
            relocateTargetStart();
            completedCurrentObject = false;
        }
        completedCurrentObject = false;

        // check whether we are close enough to the target to the end
        // this is in alternate way of ending the current task when collision is buggy
        Vector2 alternate_to = new Vector2(current.transform.position.x, current.transform.position.z);
        Vector2 alternate_from = new Vector2(CenterEyeAnchor.transform.position.x, CenterEyeAnchor.transform.position.z);
        
        Vector3 debug_to = new Vector3(alternate_to.x, 0.2f, alternate_to.y);
        Vector3 debug_from = new Vector3(alternate_from.x, 0.2f, alternate_from.y);

        debug_to = debug_from + (debug_to - debug_from).normalized * alternateDistThreshold;
        
        _lineRenderer.gameObject.SetActive(true);
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, debug_to);
        _lineRenderer.SetPosition(1, debug_from);
        
        float alternateDist = Vector2.Distance(alternate_from, alternate_to);
        if (alternateDist < alternateDistThreshold){
            completedCurrentObject = true; // move to the next object!
        }

        // Update the distance traveled
        playerDistance += Vector3.Distance(avatar.transform.position, playerLastPosition);
        playerLastPosition = avatar.transform.position;
        if (isScaled)
        {
            scaledPlayerDistance += Vector3.Distance(scaledAvatar.transform.position, scaledPlayerLastPosition);
            scaledPlayerLastPosition = scaledAvatar.transform.position;
        }

		return false;
	}

	public override void endTask()
	{
		TASK_END();
		//avatarController.handleInput = false;
	}

	public override void TASK_PAUSE()
	{
		avatarLog.navLog = false;
        if (isScaled) scaledAvatarLog.navLog = false;
		//base.endTask();
		log.log("TASK_PAUSE\t" + name + "\t" + this.GetType().Name + "\t" ,1 );
		//avatarController.stop();

		hud.setMessage("");
		hud.showScore = false;

	}

	public override void TASK_END()
	{
		base.endTask();
		//avatarController.stop();
		avatarLog.navLog = false;
        if (isScaled) scaledAvatarLog.navLog = false;

		hud.setMessage("");
		hud.showScore = false;

        hud.SecondsToShow = hud.GeneralDuration;

        // Move hud back to center and reset
        hud.hudPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        hud.hudPanel.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

        float perfDistance;
        if (isScaled)
        {
            perfDistance = scaledPlayerDistance;
        }
        else perfDistance = playerDistance;

        var excessPath = perfDistance - optimalDistance;
        var navTime = Time.time - startTime;

        // set impossible values if the nav task was skipped
        if (skip)
        {
            navTime = float.NaN;
            perfDistance = float.NaN;
            optimalDistance = float.NaN;
            excessPath = float.NaN;
        }

        log.log("LM_OUTPUT\tRelocationTask.cs\tRelocated to the asked position", 1);

    }

    private void relocateTargetStart(){
        TargetObject currTgtObj = trialData.targetObjects[currObjInd];
        Debug.Log("instantiating target in current.");
        Debug.Log(currObjInd);
        current = InstantiateRelocationTarget(currTgtObj.x, currTgtObj.y);
        current.SetActive(true);
    }

    private void relocateTargetEnd(){
        current.SetActive(false);
        current = null;
        currObjInd++;
    }

    private GameObject InstantiateRelocationTarget(float x, float y){

        // get current target!
        GameObject targetObject;

        if (currObjInd < numObjects - 1){ // load target from string in config JSON
            string targetObjectString = trialData.targetObjects[currObjInd].object_repr;
            targetObject = relocationTargetTemplateParent.transform.Find(targetObjectString).gameObject;
        } else { // load the final target template
            targetObject = relocationFinalTargetPrefab;
        }

        GameObject go = Instantiate(targetObject, currentTargetObjectHolder.transform);
        go.transform.position = new Vector3(x, 1.0f, y);
        return go;
    }

	public override bool OnControllerColliderHit(GameObject hit)
	{
        Debug.Log("OnControllerColliderHit-RelocationTask.cs");
		if (hit == current)
		{
			completedCurrentObject = true;
		}

		if (hit.transform.parent == current.transform)
		{
			completedCurrentObject = true;
		}
		return false;
	}
}
