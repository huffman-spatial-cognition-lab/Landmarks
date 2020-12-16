using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RelocationTask : ExperimentTask
{
    [Header("Task-specific Properties")]
    public ObjectList destinations;
	private GameObject current;

    public bool hideNonTargets;


    // For logging output
    private float startTime;
    private Vector3 playerLastPosition;
    private float playerDistance = 0;
    private Vector3 scaledPlayerLastPosition;
    private float scaledPlayerDistance = 0;
    private float optimalDistance;

    private bool debugOculusPosition = false;

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
		current = destinations.currentObject();
        // Debug.Log ("Find " + destinations.currentObject().name);

        hud.SecondsToShow = 0;
        hud.setMessage("Please relocate to the target");

        // Handle if we're hiding all the non-targets
        // MIGHT BE USEFUL FOR SHOWING/REMOVING BORDERS
        /*
        if (hideNonTargets)
        {
            foreach (GameObject item in destinations.objects)
            {
                if (item.name != destinations.currentObject().name)
                {
                    item.SetActive(false);
                }
                else item.SetActive(true);
            }
        }
        */

        // make sure the target is visible
        //  destinations.currentObject().SetActive(true); 
        current.SetActive(true);
        
        try
        {
            destinations.currentObject().GetComponent<MeshRenderer>().enabled = true;
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

        Debug.Log("START");
        Debug.Log("OVRPlayerController" + GameObject.Find("OVRPlayerController").transform.position);
        Debug.Log("OVRCameraRig" + GameObject.Find("OVRCameraRig").transform.position);
        Debug.Log("OVRCameraRig/TrackingSpace" + GameObject.Find("OVRCameraRig/TrackingSpace").transform.position);
        Debug.Log("TrackingSpace/LeftEyeAnchor" + GameObject.Find("TrackingSpace/LeftEyeAnchor").transform.position);
        Debug.Log("TrackingSpace/CenterEyeAnchor" + GameObject.Find("TrackingSpace/CenterEyeAnchor").transform.position);

    }

    public override bool updateTask ()
	{
		base.updateTask();

        if (debugOculusPosition){
            Debug.Log("UPDATE");
            Debug.Log("OVRPlayerController" + GameObject.Find("OVRPlayerController").transform.position);
            Debug.Log("OVRCameraRig" + GameObject.Find("OVRCameraRig").transform.position);
            Debug.Log("OVRCameraRig/TrackingSpace" + GameObject.Find("OVRCameraRig/TrackingSpace").transform.position);
            Debug.Log("TrackingSpace/LeftEyeAnchor" + GameObject.Find("TrackingSpace/LeftEyeAnchor").transform.position);
            Debug.Log("TrackingSpace/CenterEyeAnchor" + GameObject.Find("TrackingSpace/CenterEyeAnchor").transform.position);

        }

        if (skip)
        {
            //log.log("INFO    skip task    " + name,1 );
            return true;
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

        // turn off the object so that it doesn't bother us in future trials
        current.SetActive(false); 

        if (canIncrementLists)
		{
			destinations.incrementCurrent();
		}
		current = destinations.currentObject();
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

        var parent = this.parentTask;
        var masterTask = parent;
        while (!masterTask.gameObject.CompareTag("Task")) masterTask = masterTask.parentTask;

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

	public override bool OnControllerColliderHit(GameObject hit)
	{
		if (hit == current)
		{
			return true;
		}

        //Debug.Log(hit);
        //Debug.Log(current);
		if (hit.transform.parent == current.transform)
		{
			return true;
		}
		return false;
	}
}
