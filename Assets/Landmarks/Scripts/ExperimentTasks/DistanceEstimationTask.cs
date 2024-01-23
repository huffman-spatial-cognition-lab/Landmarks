/*
    This script will run a straight-line distance estimation task.

    Note: this version of the task will ask participants to press
    the space bar to move from "Imagine you're standing at " to
    get the "What is the straight-line distance to " bit of the
    question. Moreover, the participant is instructed to press
    the space bar again to input the distance. This will allow
    us to calculate the reaction time for the imagined location
    as well as the reaction time to "compute" the distance to
    the second target location. That is, we could also look at
    the RT for when they start input of numbers, but there could
    be some variability there due to moving fingers for number
    keys. Here, we can look explicitly at when the "feel ready"
    to respond, and importantly, the key press will be the same
    as the "imagined standing" bit. Also, there is a
    responseCooldown variable here, set to an initial default
    of 75 ms.

    This script was modified from my confidence rating script.

    WRITTEN BY DEREK J HUFFMAN (MARCH 2019)

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.Characters.ThirdPerson;

public class DistanceEstimationTask : ExperimentTask {

	// public ExperimentTask questionList;
	private string current;
	public TextAsset instruction;
    public TextAsset message;
	public string question = "";
	public string answer = "";
	private int landmarkCount = 1;
	private GameObject start_destination;
	private GameObject endObject;

	public TextList texts;
    private string currentText;
	private Text gui;
	private float startTime;
	public bool restrictMovement = false; 
        
    public bool blackout = true;
    public Color text_color = Color.white;
    public Font instructionFont;
    public int instructionSize = 12;

	public MoveObjects objsAndLocs;
    private Dictionary<GameObject,GameObject> objDict;
    private Dictionary<GameObject,GameObject> centerDict;

    public ExperimentTask Rooms;
	private string currentObjs;
    private string[] roomList;
    private List<GameObject> roomObjList;

	public GameObject textUI;
	protected UnityEngine.UI.Text textForQuestion;
	private long timeThisStarted;
	private bool viewable = false;
	private bool numberYet = false;
	// currently set to 0, b/c this code requires subjects
	// to have first pressed a number key before pressing
	// return; hence, false "return" is highly unlikely
	// and raw reaction times could be interesting here.
	public int responseCooldown = 75;
	// initialize a currResponse variable
	private string currResponse;
	// initialize string array to hold the imagined position and other location
	// private char[] questionStandDist;
	// initialize bool for ready for second store and ready to respond
	private bool readyForSecondStore = false;
	private bool readyToRespond = false;

	private string currAnswerQuestion;
	private string currAnswer;

	public bool withAnswer = false;
    public bool isVirtualSilcton = false;

    private string within_or_bw;

	// Use this for initialization
	void Awake () {
		textUI.SetActive(viewable);
	}


	public override void startTask () {
		TASK_START();
        Debug.Log ("Starting Absolute Distance Judgment Task");
        ResetHud();
	}


	public override void TASK_START ()
	{
		if (!manager) Start();
        base.startTask();

        startTime = Time.time;
        
        if (skip) {
            log.log("INFO    skip task    " + name,1 );
            return;
        }

		// Parse the path we are supposed to follow
        currentObjs = Rooms.currentString();

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
        start_destination = centerDict[roomObjList[0]];
        // Debug.Log(start_destination.transform.position.ToString());
        // Debug.Log(GameObject.Find("obj20"));
        GameObject endLocation = roomObjList[1];
        endObject = objDict[endLocation];
        // Debug.Log(objDict[endLocation]);


		GameObject sgo = new GameObject("Instruction Display");

        GameObject avatar = manager.player.GetComponent<HUD>().Canvas as GameObject;
        Text canvas = avatar.GetComponent<Text>();
        hud.SecondsToShow = hud.InstructionDuration;

            
        sgo.AddComponent<Text>();
        sgo.hideFlags = HideFlags.HideAndDontSave;
        sgo.transform.position = new Vector3(0,0,0);
        gui = sgo.GetComponent<Text>(); 
        // DEPRICATED IN UNITY 2019 // gui.pixelOffset = new Vector2( 20, Screen.height - 20);
        gui.font = instructionFont;
        gui.fontSize = instructionSize;
        gui.material.color = text_color;                 

        if (texts) currentText = texts.currentString().Trim();
        if (instruction) canvas.text = instruction.text;

        // Determine where we're getting the text from (default is message)
        if (message == null & texts != null)
        {
            Debug.Log("No message asset detected; texts asset found; using texts");
            gui.text = currentText;
        }
        else
        {
            Debug.Log("Attempting to use the default, message, asset.");
            gui.text = message.text;
        }

        if (blackout) hud.showOnlyHUD();

        if (message) {
            string msg = message.text;
            if (currentText != null) msg = string.Format(msg, currentText);
            // if (currentObject != null) msg = string.Format(msg, currentObject.name);
            hud.setMessage(msg);
        }
        else if (!message & texts)
        {
            string msg = currentText;
            // if (currentObject != null) msg = string.Format(msg, currentObject.name);
            hud.setMessage(msg);
        }

        hud.flashStatus("");

        if (restrictMovement)
        {
            manager.player.GetComponent<CharacterController>().enabled = false;
            manager.scaledPlayer.GetComponent<ThirdPersonCharacter>().immobilized = true;
        }




		if (withAnswer) {
			string[] parts = new string[1];		
			parts = current.Split (new char[] { '\t' });				        	

			answer = parts [0];
			question = parts [1];
            if (isVirtualSilcton)
            {
                within_or_bw = parts[2];
            }
		} else {
			question = current;
		}

		question = question.Replace("    ", "\n");   //workaround for multi line questions

		// split the string so that we can have the "standing" bit appear first
		// until the participant pressed the "Space" bar.
		// questionStandDist = question.Split(new char[] {"\n\n"});

		// make text viewable
		viewable=true;
		textUI.SetActive (viewable);
		// log the information
		log.log ("INFO\tDIST_EST_TASK_START\t" + landmarkCount + "\t" + question, 1);
		// update my new UI text
		textForQuestion = GameObject.Find ("textForTask").GetComponent<UnityEngine.UI.Text> ();
		// textForQuestion.text = questionStandDist[0] + "\n\n\n\n\n\n\n\n"; 

		// update this variable, which is used for timing/responseCooldown purposes
		timeThisStarted = Experiment.Now ();
	}


	public override bool updateTask ()
	{
		// Ask participants to rate their agreement; once participant has
		// pressed keys 1 through 7, change the font to yellow for their selected
		// response. Once they press return (if the reponseCooldown has passed),
		// bail out of this task. Note: the yellow text is set with:
		// "<color=yellow>MY YELLOW TEXT</color>"

		// Check if they've pressed for the second part of question
		if (!readyForSecondStore && Input.GetKeyDown(KeyCode.Space)) {
			log.log("INFO\tDIST_EST_RESP_STANDING\t" + landmarkCount + "\t" + question, 1);
			timeThisStarted = Experiment.Now();
			readyForSecondStore = true;
			textForQuestion.text = question + "\n\n\n\n\n"; 
		}

		// Check if they've pressed to indicate that they're ready to respond
		if (readyForSecondStore && !readyToRespond && Input.GetKeyDown(KeyCode.Space)) {
			// check if the responseCooldown has passed
			if (Experiment.Now () >= timeThisStarted + responseCooldown) {
				log.log ("INFO\tDIST_EST_READY_TO_RESPOND\t" + landmarkCount + "\t" + question, 1);
				readyToRespond = true;
			}
		}

		// If they've already pressed for second part of question, then accept input
		if (readyToRespond) {
			if (numberYet && Input.GetButtonDown ("Return") && Experiment.Now () >= timeThisStarted + responseCooldown) {
				// they've selected their response and the responseCooldown window has passed
				//Debug.Log ("Response has been made!");
				// break free
				return true;
			} else {
				if ((Input.inputString.ToString ().Length == 1) && char.IsNumber (Input.inputString.ToString () [0])) {
					// Pressed a number, so add it to the cue
					currResponse = currResponse + Input.inputString.ToString ();
					numberYet = true;
					textForQuestion.text = question + "\n\n" + currResponse + "\n\n\n";
					log.log ("INFO\tDIST_EST_INPUT\t" + landmarkCount + "\t" + currResponse, 1);
				} else if (Input.GetKeyDown (KeyCode.Period)) {
					// Pressed period, so add that (e.g., for decimal point)
					currResponse = currResponse + ".";
					numberYet = false;
					textForQuestion.text = question + "\n\n" + currResponse + "\n\n\n";
					log.log ("INFO\tDIST_EST_INPUT\t" + landmarkCount + "\t" + currResponse, 1);
				} else if (numberYet && Input.GetKeyDown (KeyCode.Backspace)) {
					// Pressed Backspace, so remove the last element
					if (currResponse.Length == 1) {
						currResponse = "";
						numberYet = false;
						textForQuestion.text = question + "\n\n\n\n\n"; 
					} else {
						currResponse = currResponse.Substring (0, currResponse.Length - 1);
						textForQuestion.text = question + "\n\n" + currResponse + "\n\n\n";
					}
					log.log ("INFO\tDIST_EST_INPUT_BACKSPACE\t" + landmarkCount + "\t" + currResponse, 1);
				}
			}
		}
		// default is to keep running
		return false;
	}


	public override void endTask() {
		TASK_END();
	}


    public override void TASK_END()
    {
        base.endTask();

        // log their response (see updateTask code for how currResponse is set)
        if (withAnswer) {
            if (isVirtualSilcton) {
                // here I will also log whether it is a within or between route trial
                log.log("INFO\tDIST_EST_TASK_END\t" + landmarkCount + "\t" + question.Replace("\n", "    ") + "\t" + currResponse + "\t" + answer + "\t" + within_or_bw, 1);
            } else {
                log.log("INFO\tDIST_EST_TASK_END\t" + landmarkCount + "\t" + question.Replace("\n", "    ") + "\t" + currResponse + "\t" + answer, 1);
            }
		} else {
			log.log ("INFO\tDIST_EST_TASK_END\t" + landmarkCount + "\t" + question.Replace ("\n", "    ") + "\t" + currResponse, 1);
		}

		// clear and deactivate the textUI (just in case)
		textForQuestion.text = "";
		viewable = false;
		textUI.SetActive(viewable);

        hud.setMessage ("");
        hud.SecondsToShow = hud.GeneralDuration;

		// reset to false
		numberYet = false;
		readyForSecondStore = false;
		readyToRespond = false;


		landmarkCount++;

		// reset to empty string
		currResponse = "";
	}

}
