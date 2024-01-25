/*
    This class will run the SelectItems task.

    Initial starting point:
        MapTestTask.cs (by Michael J. Starrett and Jared D. Stokes)
		SelectItems.cs (by  Derek J. Huffman, Summer 2021)
		ViewTargets.cs (by Michael J. Starrett)
	
	Written by Ainsley Bonin, Fall 2023
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Characters.FirstPerson;

public class RDJ : ExperimentTask
{
	[HideInInspector] public GameObject start;
	public bool scaledPlayer = false;
	[Header("Task-specific Properties")]

	public bool blackout;
	[HideInInspector] public GameObject destination;

	public ExperimentTask Objs;
	public string currentTrial;
    private string[] objList;
	public MoveObjects objsAndLocs;
    private Dictionary<GameObject,GameObject> objDict;
	private GameObject target1;
	private GameObject target2;
	public List<GameObject> targetList;

	public ObjectList TargetObjects;
	public ObjectList Doors;

	// public CopyObjectsAB copyObjects;
	[Tooltip("In seconds; 0 = unlimited time")]
	public int timeLimit = 0;
	// public bool flattenMap = true;
	//public bool highlightAssist = false; // MJS - Removing Target Highlights for ease of use (requires additional environment configuration)
	//public GameObject mapTestHighlights; // MJS - Removing Target Highlights for ease of use (requires additional environment configuration)
	public float snapToTargetProximity = 0.0f; // leave at 0.0f to have snapping off. Otherwise this will be the straight line distance within a target users must be to snap object to target position/location
	[TextArea]
	public string buttonText = "These objects are the same distance from my starting point";
	// public string doneButtonText = "Get Score";

	// AKB - lists to store original properties of objects
	private List<Vector3> position;
	private List<Quaternion> rotation;
	private List<Transform> parent;
	private List<Vector3> scale;
	public Vector3 objectPositionOffset;
	public Vector3 objectRotationOffset;

	private GameObject targetObjects; // should be the game object called TargetObjects under Environment game object
	// [HideInInspector] public List<GameObject> destinations;
	private int saveLayer;
	private int viewLayer = 11;

	private bool[] targetInBoundsList;

	private GameObject activeTarget; // This is the container we will use for whichever object is currently being clicked and dragged
	private bool targetActive = false; // Are we currently manipulating a targetobject?
	// private Vector3 previousTargetPos; // save the position when a target was clicked so users can undo the current move
	// private Vector3 previousTargetRot; // save the rotation when a target was clicked so users can undo the current rotate
									   // allow for user input to shift the store labels during the map task (to allow viewing store and text clearly);
	public Vector3 hudTextOffset; // Text will be centered over an object. This allows users to move that to a desireable distance in order to keep the object visible when the name is shown

	private Vector3 baselineScaling;

	private float startTime;
	private float taskDuration;

	// DJH - new variables for select tasks
	public int numTargsToSelect = 1;  // total number of targets you want participants to select
	public float timeShowInBoundsMessage = 2.5f;  // the amount of time (seconds) to show in bounds message
	private float buttonPressTime;
	public float envCenterX = 0.0f;  // the center of the SelectItems part of your environment
	public float envCenterZ = 0.0f;  // same as above, but for the Z dimension
	public float offsetX = 0.0f;  // if you want an offset for showing the message for number of items in bounds
	public float offsetZ = 0.0f;  // same as above, but for the Z dimension
	private float minX = 1010f;  // the maximum X value
	private float maxX = 1025f;  // the minumum X value
	private float minZ = 955f;  // the maximum Z value
	private float maxZ = 1025f;  // the minumum Z value

	public bool restrictMovement = true;
	private Vector3 initialHUDposition;

	public override void startTask()
	{
		TASK_START();
		Debug.Log("RDJ task is starting");
		initTargets();
		avatarLog.navLog = false;
	}


	public override void TASK_START()
	{
		if (!manager) Start();
		base.startTask();

		if (blackout) hud.showOnlyTargets();
	    else hud.showEverything();

		startTime = Time.time;
		// DJH - adding for button press
		buttonPressTime = Time.time;

		// Modify the HUD display for the map task
		hud.setMessage("");
		hud.hudPanel.SetActive(true); // temporarily turn off the hud panel at task start (no empty message window)
		hud.ForceShowMessage();
		// move hud off screen if we aren't hitting a target shop
		hud.hudPanel.transform.position = new Vector3(99999, 99999, 99999);

		// make the cursor functional and visible
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		// Turn off Player movement
		avatar.GetComponent<CharacterController>().enabled = false;

		// Swap from 1st-person camera to overhead view
		// firstPersonCamera.enabled = false;
		// overheadCamera.enabled = true;

		// Change text and turn on the map action button
		actionButton.GetComponentInChildren<Text>().text = buttonText;
		hud.actionButton.SetActive(true);
		hud.actionButton.GetComponent<Button>().onClick.AddListener(hud.OnActionClick);

		firstPersonCamera.enabled = false;
		overheadCamera.enabled = true;

		if (restrictMovement)
        {
            manager.player.GetComponent<CharacterController>().enabled = false;
			if (avatar.GetComponent<FirstPersonController>())
			{
				avatar.GetComponentInChildren<Camera>().transform.localEulerAngles = Vector3.zero;
				avatar.GetComponent<FirstPersonController>().ResetMouselook();
				avatar.GetComponent<FirstPersonController>().enabled = false;
			}
			manager.scaledPlayer.GetComponent<ThirdPersonCharacter>().immobilized = true;
        }

		// destination = avatar.GetComponentInChildren<LM_SnapPoint>().gameObject;
		destination = new GameObject("Destination");
		destination.transform.parent = transform;
		destination.transform.localPosition = new Vector3(envCenterX,1.5f,envCenterZ);


        // handle changes to the hud
        if (vrEnabled)
        {
            initialHUDposition = hud.hudPanel.transform.position;

            var temp = destination.transform.position;
            temp.y += 2.5f;
            hud.hudPanel.transform.position = temp;

        }
        else
        {
            // Change the anchor points to put the message at the bottom
            RectTransform hudposition = hud.hudPanel.GetComponent<RectTransform>() as RectTransform;
            hudposition.pivot = new Vector2(0.5f, 0.1f);
        }


		// Grab target objects
        currentTrial = Objs.currentString();

        objList = currentTrial.Split(new char[] {','});

		GameObject loc1 = GameObject.Find(objList[0]);
		GameObject loc2 = GameObject.Find(objList[1]);

		objDict = objsAndLocs.objDict;
		target1 = objDict[loc1];
		target2 = objDict[loc2];
		targetList.Add(target1);
		targetList.Add(target2);

		// Turn off all other target objects and doors
        foreach (GameObject item in TargetObjects.objects)
        {
            item.SetActive(false);
        }

        foreach (GameObject door in Doors.objects)
        {
            door.SetActive(false);
        }

		Debug.Log("Before turning on objects");
		// Turn on copyObjects
		// foreach (GameObject copy in copyObjects.copies) {
		// 	Debug.Log("here");
		// 	copy.SetActive(true);
		// 	Debug.Log(copy.transform.position);
		// }


	}

	// // AKB question - do I need this function? Or no since we have a list of copyObjects?
	public void initTargets() {
		
		int i = 0;
		position = new List<Vector3>();
		rotation = new List<Quaternion>();
		parent = new List<Transform>();
		scale = new List<Vector3>();


		foreach (GameObject target in targetList) {
			Debug.Log(target);
			// store original properties of the target
			position.Add(target.transform.position);
			rotation.Add(target.transform.rotation);
			parent.Add(target.transform.parent);
			scale.Add(target.transform.localScale);

			// move the target to the viewing location temporarily
			target.transform.parent = destination.transform;

			if (i == 0) {
				objectPositionOffset.x = 5;
			} else {
				objectPositionOffset.x = -5;
			}

			target.transform.localPosition = objectPositionOffset;
			target.transform.localEulerAngles = objectRotationOffset;
			target.transform.localScale = Vector3.Scale(target.transform.localScale, destination.transform.localScale);

			target.transform.parent = parent[i];

			target.SetActive(true);
			i += 1;

			saveLayer = target.layer;				// AKB question - do I need these two lines?
			setLayer(target.transform,viewLayer);

			log.log("RDJ\t" + target.name,1);
			// Debug.Log(target.transform.position);
			// Debug.Log(target.tag);

		}
		
	}


	public void setLayer(Transform t, int l) {
		t.gameObject.layer = l;
		foreach (Transform child in t) {
			setLayer(child,l);
		}
	}


	public override bool updateTask()
	{
		base.updateTask();

		taskDuration = Time.time - startTime;

		// If we're using a time limit; end the task when time is up
		if (timeLimit > 0 & taskDuration > timeLimit)
		{
			return true;
		}

		// ------------------------------------------------------------
		// Handle mouse input for hovering over and selecting objects
		// ------------------------------------------------------------

		// create a plane for our raycaster to hit
		// Note: make y high enough that store is visible over other envir. buildings
		Plane plane = new Plane(Vector3.up, new Vector3(0, 0.2f, 0));

		//empty RaycastHit object which raycast puts the hit details into
		RaycastHit hit;

		//ray shooting out of the camera from where the mouse is
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 10);

		// Register when our raycaster is hitting a gameobject...
		if (Physics.Raycast(ray, out hit, 100))
		{
			Debug.Log(hit.collider.gameObject.transform.position);
			// ... but only if that game object is one of our target stores ...
			if (hit.collider.gameObject == targetList[0]) {
				Debug.Log(targetList[0].name);
				hud.setMessage(hit.transform.name);
				hud.hudPanel.SetActive(true);
				hud.ForceShowMessage();

				// move hud text to the store being highlighted (coroutine to prevent Update framerate jitter)
				// jitterGuardOn is inherited from Experiment task so it can be used in multiple task scripts (e.g., MapStudy and MapTest) - MJS 2019
				if (!jitterGuardOn)
				{
					hud.hudPanel.transform.position = Camera.main.WorldToScreenPoint(hit.transform.position + hudTextOffset);
					StartCoroutine(HudJitterReduction());
				}

				log.log("Mouseover \t" + hit.transform.name, 1);


				// BEHAVIOR Click Store to make it follow mouse
				if (Input.GetMouseButtonDown(0))
				{
					// Container for active store
					targetActive = true;
					activeTarget = hit.transform.gameObject;
					// Record previos position so the current move can be cancelled
					// previousTargetPos = activeTarget.transform.position;
					// previousTargetRot = activeTarget.transform.eulerAngles;
				}
			}
			else if (hit.transform.name == targetList[1].name)
			{
				hud.setMessage(hit.transform.parent.transform.name);
				hud.hudPanel.SetActive(true);
				hud.ForceShowMessage();

				// move hud text to the store being highlighted (coroutine to prevent Update framerate jitter)
				// jitterGuardOn is inherited from Experiment task so it can be used in multiple task scripts (e.g., MapStudy and MapTest) - MJS 2019
				if (!jitterGuardOn)
				{
					hud.hudPanel.transform.position = Camera.main.WorldToScreenPoint(hit.transform.parent.transform.position + hudTextOffset);
					StartCoroutine(HudJitterReduction());
				}

				log.log("Mouseover \t" + hit.transform.parent.transform.name, 1);

				// BEHAVIOR Click Store to make it follow mouse
				if (Input.GetMouseButtonDown(0))
				{
					// Container for active store
					targetActive = true;
					activeTarget = hit.transform.parent.transform.gameObject;
					// Record previos position so the current move can be cancelled

				}
			}
		}
	
			// ... Otherwise, clear the message and hide the gui
		// 	else if (Time.time - buttonPressTime > timeShowInBoundsMessage)
		// 	{
		// 		HideStoreName();
		// 	}
		// }
		// else if (Time.time - buttonPressTime > timeShowInBoundsMessage)
		// {
		// 	HideStoreName();
		// }

		// -----------------------------------------
		// Manipulate the currently active store
		// -----------------------------------------

		if (targetActive)
		{
			log.log("Options :\t" + targetList, 1);
			log.log("Selected :\t" + activeTarget.name, 1);
			// AKB: if I return true, will I just end the trial here?
			return true;

		}

		// -----------------------------------------
		// Handle debug button behavior (kill task)
		// -----------------------------------------
		if (killCurrent == true)
		{
			return KillCurrent();
		}

		// -----------------------------------------
		// Handle action button behavior
		// -----------------------------------------
		if (hud.actionButtonClicked == true)		// participant is saying the obj are the same distance
		{
			hud.actionButtonClicked = false;
			log.log("Options :\t" + targetList, 1);
			log.log("Selected :\t" + "SAME DISTANCE", 1);
			return true;
		}
		return false;
	}

	public void returnTargets() {

		int i = 0;
		foreach (GameObject target in targetList) {
			target.transform.position = position[i];
			target.transform.localRotation = rotation[i];
			target.transform.localScale = scale[i];
			setLayer(target.transform,saveLayer);

			// turn off the object we're returning before turning on the next one
			target.SetActive(false);
			i += 1;
		}
		
    }



	public override void endTask()
	{
		returnTargets();
		TASK_END();
	}


	public override void TASK_END()
	{
		base.endTask();

		// Log data
		trialLog.AddData(transform.name + "_testTime", taskDuration.ToString());

		// Set up hud for other tasks
		hud.hudPanel.SetActive(true); //hide the text background on HUD
									  // Change the anchor points to put the message back in center
		RectTransform hudposition = hud.hudPanel.GetComponent<RectTransform>() as RectTransform;
		hudposition.anchorMin = new Vector2(0, 0);
		hudposition.anchorMax = new Vector2(1, 1);
		hudposition.pivot = new Vector2(0.5f, 0.5f);
		hudposition.anchoredPosition3D = new Vector3(0, 0, 0);
		//hud.hudPanel.GetComponent<RectTransform> = hudposition;


		// make the cursor invisible
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false;

		// Turn on Player movement
		avatar.GetComponent<CharacterController>().enabled = true;

		// Swap from overhead camera to first-person view
		firstPersonCamera.enabled = true;
		overheadCamera.enabled = false;


		// turn off the map action button
		hud.actionButton.GetComponent<Button>().onClick.RemoveListener(hud.OnActionClick);
		actionButton.GetComponentInChildren<Text>().text = actionButton.GetComponent<DefaultText>().defaultText;
		hud.actionButton.SetActive(false);

		// MJS - Removing Target Highlights for ease of use (requires additional environment configuration)
		//// Turn off the maptarget highlights (to show where stores should be located
		//if (highlightAssist == true)
		//{
		//	mapTestHighlights.SetActive (false);
		//}

		// DJH - Copying from MapScoreTest.cs
		// -------------------------------
		// Prep the Target Object States
		// -------------------------------

		// Destroy the copies we created when initializing the map test task
		// foreach (Transform child in copyObjects.transform)
		// {
		// 	Destroy(child.gameObject);
		// }

		returnTargets();

		// DJH - Also activate the original objects
		// reactivate the original objects
		// targetObjects = copyObjects.GetComponent<CopyChildObjects>().sourcesParent.parentObject; // should be the game object called TargetObjects under Environment game object
		foreach (GameObject item in TargetObjects.objects)
        {
            item.SetActive(true);
        }

        foreach (GameObject door in Doors.objects)
        {
            door.SetActive(true);
        }
		

		// DJH - Destroy game objects that they did not select
		// int counter = 0;
		// foreach (Transform child in targetObjects.transform)
		// {
		// 	if (targetInBoundsList[counter] == false)
		// 	{
		// 		Destroy(child.gameObject);
		// 	}
		// 	counter++;
		// }

	}

}

