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

public class InputParticipantID : ExperimentTask {

    [Header("Task-specific Properties")]

    public TextAsset message;
    private string current;
    protected string textForQuestion;
    
    public TextList texts;
    private string currentText;
    public int participantID;

    public ObjectList TargetObjects;
    private GameObject currentObject;
    
    public float envCenterX = 0.0f;  // the center of the SelectItems part of your environment
	public float envCenterZ = 0.0f;  // same as above, but for the Z dimension
    private int saveLayer;
	private int viewLayer = 11;
        
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

    public ObjectList env;

    private Vector3 initialHUDposition;

    
    public override void startTask () {
        TASK_START();
        Debug.Log ("Starting Participant ID Input");
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


        initialHUDposition = hud.hudPanel.transform.position;
        var tempPos = initialHUDposition;
        tempPos.y += 275;
        tempPos.x += 10;
        hud.hudPanel.transform.position = tempPos;


        firstPersonCamera.enabled = false;
		overheadCamera.enabled = true;


        string msg = message.text;
        hud.setMessage(msg);
        

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
            log.log ("INFO\tID_INPUT\t" + currResponse, 1);
            Debug.Log ("INFO\tID_INPUT\t" + currResponse);
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
            log.log ("INFO\tID_INPUT_BACKSPACE\t" + currResponse, 1);
            Debug.Log ("INFO\tID_INPUT_BACKSPACE\t" + currResponse);

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


    
    public override void endTask() {
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
        // var parent = this.parentTask;
        // var masterTask = parent;
        // while (!masterTask.gameObject.CompareTag("Task"))
        // {
            
        //     masterTask = masterTask.parentTask;
        //     Debug.Log(masterTask.name);
        // }
        var rt = Time.time - startTime;

        Debug.Log("RT: " + rt);
        Debug.Log("Response: " + currResponse);
        Debug.Log("Text for question: " + textForQuestion);
        // Output log for this task in tab delimited format
        participantID = int.Parse(currResponse);
        Debug.Log(participantID);
        log.log("TASK_INFO\tPARTICIPANT ID:\t" + participantID, 1);

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

        // if (trialLog.active)
        // {

            
        //     trialLog.AddData(transform.name + "_question", question);

        //     trialLog.AddData(transform.name + "_distanceJudge", currResponse);

        //     trialLog.AddData(transform.name + "_rt", rt.ToString());

        // }

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

        // Turn off all objects in Map Environment
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
