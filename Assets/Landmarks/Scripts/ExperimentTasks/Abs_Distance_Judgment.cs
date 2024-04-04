/*
    Copyright (C) 2018 Michael James Starrett

    Navigate by Starlite - Powered by Landmarks

    This is modified from LM_Slider_Question.cs to be a judgment of 
    distance between two landmarks
*/   

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Characters.FirstPerson;

public class Abs_Distance_Judgment : ExperimentTask {

    [Header("Task-specific Properties")]

    public TextAsset message;
    private string current;
    protected string textForQuestion;

    public MoveObjects objsAndLocs;
    private Dictionary<GameObject,GameObject> objDict;
    private Dictionary<GameObject,GameObject> centerDict;

    public ExperimentTask Rooms;
	private string currentObjs;
    private string[] roomList;
    private List<GameObject> roomObjList;
    private GameObject obj1;
	private GameObject obj2;
    
    public ObjectList TargetObjects;
    public ObjectList objects;
    private GameObject currentObject;
    
    public TextList texts;
    private string currentText;

    // For storing properties of the targets
    [HideInInspector] public GameObject destination1;
    [HideInInspector] public GameObject destination2;
    private static Vector3 position1;
	private static Quaternion rotation1;
	private static Transform parent1;
	private static Vector3 scale1;
    private static Vector3 position2;
	private static Quaternion rotation2;
	private static Transform parent2;
	private static Vector3 scale2;
    public Vector3 objectRotationOffset;
    public Vector3 objectPositionOffset;
    public float envCenterX = 0.0f;  // the center of the SelectItems part of your environment
	public float envCenterZ = 0.0f;  // same as above, but for the Z dimension
    private int saveLayer;
	private int viewLayer = 11;
    public ObjectList env;
        
    public bool blackout = true;
    public Color text_color = Color.white;
    public Font instructionFont;
    public int instructionSize = 12;

    public bool actionButtonOn = true;
    public string customButtonText = "";

    public bool restrictMovement = true; 

    private bool viewable = false;
    private string[] questionStandDist;

    private float startTime;
    public bool withAnswer = false;
    public string question = "";
	public string answer = "";
    private bool numberYet = false;
    private string currResponse;

    private Vector3 initialHUDposition;

    
    public override void startTask () {
        TASK_START();
        Debug.Log ("Starting Absolute Distance Judgment Task");
        initCurrent();
        ResetHud();
    }    

    public override void TASK_START()
    {
        
        if (!manager) Start();
        base.startTask();

        startTime = Time.time;

        if (blackout) hud.showOnlyTargets();
	    else hud.showEverything();
        
        if (skip) {
            log.log("INFO    skip task    " + name,1 );
            return;
        }

        destination1 = new GameObject("Destination");
		destination1.transform.parent = transform;
		destination1.transform.localPosition = new Vector3(envCenterX,0.0f,envCenterZ);

        destination2 = new GameObject("Destination");
		destination2.transform.parent = transform;
		destination2.transform.localPosition = new Vector3(envCenterX,0.0f,envCenterZ);

        // Parse the path we are supposed to follow
        currentObjs = Rooms.currentString();
        Debug.Log(currentObjs);

        roomList = currentObjs.Split(new char[] {','});
        roomObjList = new List<GameObject>();
        foreach (string roomName in roomList) {
    
            if (string.IsNullOrWhiteSpace(roomName)) {
                break;

            } else {

                GameObject temp = GameObject.Find(roomName);
                roomObjList.Add(temp);
            }
            
        }

        objDict = objsAndLocs.objDict;
        centerDict = objsAndLocs.centerDict;
        obj1 = objDict[roomObjList[0]];
        Debug.Log(roomObjList[0]);
        GameObject endLocation = roomObjList[1];
        obj2 = objDict[endLocation];
        Debug.Log(roomObjList[1]);

        initialHUDposition = hud.hudPanel.transform.position;
        var tempPos = initialHUDposition;
        tempPos.y += 275;
        tempPos.x += 10;
        hud.hudPanel.transform.position = tempPos;


        firstPersonCamera.enabled = false;
		overheadCamera.enabled = true;


        if (message) {
            string msg = message.text;
            if (currentText != null) msg = string.Format(msg, currentText);
            if (currentObject != null) msg = string.Format(msg, currentObject.name);
            hud.setMessage(msg);
        }
        else if (!message & texts)
        {
            string msg = currentText;
            if (currentObject != null) msg = string.Format(msg, currentObject.name);
            hud.setMessage(msg);

        }

        question = message.text;


        hud.flashStatus("");

        if (restrictMovement)
        {
            manager.player.GetComponent<CharacterController>().enabled = false;
            manager.scaledPlayer.GetComponent<ThirdPersonCharacter>().immobilized = true;
        }

        // Change text and turn on the map action button if we're using it
        if (actionButtonOn)
        {
            // Use custom text for button (if provided)
            if (customButtonText != "") actionButton.GetComponentInChildren<Text>().text = customButtonText;
            // Otherwise, use default text attached to the button (component)
            else actionButton.GetComponentInChildren<Text>().text = actionButton.GetComponent<DefaultText>().defaultText;

            // activate the button
            hud.actionButton.SetActive(true);
            hud.actionButton.GetComponent<Button>().onClick.AddListener(hud.OnActionClick);

            // make the cursor functional and visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        foreach (GameObject item in TargetObjects.objects)
        {
            item.SetActive(false);
        }

        // Turn off all objects in Map Environment

		foreach (GameObject envObj in env.objects) {
			envObj.SetActive(false);
		}


		// split the string so that we can have the "standing" bit appear first
		// until the participant pressed the "Space" bar.
		// questionStandDist = question.Split(new char[] {"\n\n"});
        textForQuestion = question + "\n\n";
        currResponse = "";

    }

    public void initCurrent() {
		// store original properties of the target
		position1 = obj1.transform.position;
        rotation1 = obj1.transform.rotation;
		parent1 = obj1.transform.parent;
		scale1 = obj1.transform.localScale;

        Quaternion localRot1 = obj1.transform.localRotation;

		// move the target to the viewing location temporarily
		obj1.transform.parent = destination1.transform;
        objectPositionOffset.x = 3;
		obj1.transform.localPosition = objectPositionOffset;
        obj1.transform.localRotation = localRot1;
        // obj1.transform.localEulerAngles = objectRotationOffset;
        // obj1.transform.localScale = Vector3.Scale(obj1.transform.localScale, destination1.transform.localScale);

		// return the target to its original parent (we'll revert other values later)
		// this way it won't track with the "head" of the avatar
		obj1.transform.parent = parent1;

        // store original properties of the target
		position2 = obj2.transform.position;
        rotation2 = obj2.transform.rotation;
		parent2 = obj2.transform.parent;
		scale2 = obj2.transform.localScale;

        Quaternion localRot2 = obj2.transform.localRotation;

		// move the target to the viewing location temporarily
		obj2.transform.parent = destination2.transform;
        objectPositionOffset.x = -3;
        // obj2.transform.localEulerAngles = objectRotationOffset;
		obj2.transform.localPosition = objectPositionOffset;
        obj2.transform.localRotation = localRot2;
        // obj2.transform.localScale = Vector3.Scale(obj2.transform.localScale, destination2.transform.localScale);

		// return the target to its original parent (we'll revert other values later)
		// this way it won't track with the "head" of the avatar
		obj2.transform.parent = parent2;

        // but Turn on the current object
        obj1.SetActive(true);
        obj2.SetActive(true);

        saveLayer = obj1.layer;
		setLayer(obj1.transform,viewLayer);

        Debug.Log(obj1.transform.position);
        // hud.setMessage(current.name);
        // hud.ForceShowMessage();
		
		log.log("ADJ\t" + obj1.name,1);
        log.log("ADJ\t" + obj2.name,1);

	}

    public override void TASK_ADD(GameObject go, string txt) {
		if (txt == "add") {
			saveLayer = go.layer;
			setLayer(go.transform,viewLayer);
		} else if (txt == "remove") {
			setLayer(go.transform,saveLayer);
		}

	}

    // Update is called once per frame
    public override bool updateTask () {
        
        if (skip) {
            //log.log("INFO    skip task    " + name,1 );
            return true;
        }
        if ( interval > 0 && Experiment.Now() - task_start >= interval)  {
            return true;
        }

        if ((Input.inputString.ToString ().Length == 1) && char.IsNumber (Input.inputString.ToString () [0])) {
            // Pressed a number, so add it to the cue
            currResponse = currResponse + Input.inputString.ToString ();
            numberYet = true;
            textForQuestion = question + "\n" + currResponse + "\n";
            log.log ("INFO\tDIST_EST_INPUT\t" + currResponse, 1);
            Debug.Log ("INFO\tDIST_EST_INPUT\t" + currResponse);
        } else if (Input.GetKeyDown (KeyCode.Period)) {
            // Pressed period, so add that (e.g., for decimal point)
            currResponse = currResponse + ".";
            numberYet = false;
            textForQuestion = question + "\n" + currResponse + "\n";
            log.log ("INFO\tDIST_EST_INPUT\t" + currResponse, 1);
            Debug.Log ("INFO\tDIST_EST_INPUT\t" + currResponse);
        } else if (numberYet && Input.GetKeyDown (KeyCode.Backspace)) {
            // Pressed Backspace, so remove the last element
            if (currResponse.Length == 1) {
                currResponse = "";
                numberYet = false;
                textForQuestion = question + "\n\n"; 
            } else {
                currResponse = currResponse.Substring (0, currResponse.Length - 1);
                textForQuestion = question + "\n" + currResponse + "\n";
            }
            log.log ("INFO\tDIST_EST_INPUT_BACKSPACE\t" + currResponse, 1);
            Debug.Log ("INFO\tDIST_EST_INPUT_BACKSPACE\t" + currResponse);

        }

        hud.setMessage(textForQuestion);
        hud.ForceShowMessage();

        //------------------------------------------
        // Handle buttons to advance the task - MJS
        //------------------------------------------
        if (hud.actionButtonClicked == true)
        {
            hud.actionButtonClicked = false;
            log.log("INPUT_EVENT    clear text    1", 1);
            if (!string.IsNullOrWhiteSpace(currResponse)) {
                return true;
            } 
            
        }

        if (killCurrent == true)
        {
            return KillCurrent();
        }

        return false;
    }

    public void returnCurrent() {
		obj1.transform.position = position1;
		obj1.transform.localRotation = rotation1;
		obj1.transform.localScale = scale1;

        obj2.transform.position = position2;
		obj2.transform.localRotation = rotation2;
		obj2.transform.localScale = scale2;
		setLayer(obj1.transform,saveLayer);

        // turn off the object we're returning before turning on the next one
        obj1.SetActive(false);
        obj2.SetActive(false);
    }
    
    public override void endTask() {
        returnCurrent();
        TASK_END();
    }
    
    public override void TASK_END() {
        base.endTask ();
        hud.setMessage ("");
        hud.SecondsToShow = hud.GeneralDuration;


        // -----------------------
        // Log Trial info
        // -----------------------

        // Get the parent and grandparent task to provide context in log file
        var parent = this.parentTask;
        var masterTask = parent;
        while (!masterTask.gameObject.CompareTag("Task"))
        {
            
            masterTask = masterTask.parentTask;
            Debug.Log(masterTask.name);
        }
        var rt = Time.time - startTime;

        Debug.Log("RT: " + rt);
        Debug.Log("Response: " + currResponse);
        Debug.Log("Text for question: " + textForQuestion);
        // Output log for this task in tab delimited format
        log.log("LM_OUTPUT\tAbs_Distance_Judgment.cs\t" + masterTask.name + "\t" + this.name + "\n" +
               "Task\tBlock\tTrial\tObject1\tObject2\tDistanceJudge\tRT\n" +
               masterTask.name + "\t" + masterTask.repeatCount + "\t" + parent.repeatCount + "\t" + obj1.name + "\t" + obj2.name + "\t" + currResponse + "\t" + rt
               , 1);

		viewable = false;
		// reset to false
		numberYet = false;

		// reset to empty string
		currResponse = "";

        // Set up hud for other tasks
		hud.hudPanel.SetActive(true); //hide the text background on HUD
									  // Change the anchor points to put the message back in center
		RectTransform hudposition = hud.hudPanel.GetComponent<RectTransform>() as RectTransform;
		hudposition.anchorMin = new Vector2(0, 0);
		hudposition.anchorMax = new Vector2(1, 1);
		hudposition.pivot = new Vector2(0.5f, 0.5f);
		hudposition.anchoredPosition3D = new Vector3(0, 0, 0);

        // make the cursor invisible
		// Cursor.lockState = CursorLockMode.Confined;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		// Turn on Player movement
		avatar.GetComponent<CharacterController>().enabled = true;

		// Swap from overhead camera to first-person view
		firstPersonCamera.enabled = true;
		overheadCamera.enabled = false;

        if (trialLog.active)
        {

            
            trialLog.AddData(transform.name + "_question", question);

            trialLog.AddData(transform.name + "_distanceJudge", currResponse);

            trialLog.AddData(transform.name + "_rt", rt.ToString());

        }


        // increment Rooms
        Rooms.incrementCurrent();


        if (actionButtonOn)
        {
            // Reset and deactivate action button
            actionButton.GetComponentInChildren<Text>().text = actionButton.GetComponent<DefaultText>().defaultText;
            hud.actionButton.GetComponent<Button>().onClick.RemoveListener(hud.OnActionClick);
            hud.actionButton.SetActive(false);

            // make the cursor invisible
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        foreach (GameObject item in TargetObjects.objects)
        {
            item.SetActive(true);
        }

        // // Turn off all objects in Map Environment
		foreach (GameObject envObj in env.objects) {
			envObj.SetActive(true);
		}

        // If we turned movement off; turn it back on
        if (restrictMovement)
        {
            manager.player.GetComponent<CharacterController>().enabled = true;
            if (avatar.GetComponent<FirstPersonController>())
			{
				// avatar.GetComponentInChildren<Camera>().transform.localEulerAngles = Vector3.zero;
				avatar.GetComponent<FirstPersonController>().ResetMouselook();
				avatar.GetComponent<FirstPersonController>().enabled = true;
			}
            manager.scaledPlayer.GetComponent<ThirdPersonCharacter>().immobilized = false;
        }
    }

    public void setLayer(Transform t, int l) {
		t.gameObject.layer = l;
		foreach (Transform child in t) {
			setLayer(child,l);
		}
	}

}
