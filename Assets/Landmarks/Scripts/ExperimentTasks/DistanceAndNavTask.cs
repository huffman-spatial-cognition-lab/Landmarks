using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public enum HideTargetOnStart
{
    Off,
    SetInactive,
    SetInvisible,
    SetProbeTrial
}

public class DistanceAndNavTask : ExperimentTask
{
    [HideInInspector] public GameObject start;
    [Header("Task-specific Properties")]
    // public ObjectList targetObjects;
    public ExperimentTask Doors;
    private GameObject[] doors;
    private GameObject door;
    private string[] doorList;
    private string currentDoor;

    public ObjectList destinations;
    public MoveObjects objsAndLocs;
    private Dictionary<GameObject,GameObject> objDict;
    private Dictionary<GameObject,GameObject> centerDict;

    public ExperimentTask Rooms;
	private string currentPath;
    private string[] roomList;
    private List<string> roomObjList;
    // private GameObject roomObj;
    private GameObject start_destination;
    private static Vector3 position;
	private static Vector3 rotation;

    public bool randomRotation;
	public bool scaledPlayer = false;
    public bool ignoreY = false;

    private GameObject endObject;
    private GameObject[] targets;

    public bool shortcut;   // whether this is the shortcut task or not
    public string buttonText = "Done";
    private GameObject startCue;

	private int score = 0;
	public int scoreIncrement = 50;
	public int penaltyRate = 2000;

    private float penaltyTimer = 0;

	public bool showScoring;
	public TextAsset NavigationInstruction;

    public HideTargetOnStart hideTargetOnStart;
    [Min(0)] public float showTargetAfterSeconds;
    public bool hideNonTargets;

    // for compass assist
    public LM_Compass assistCompass;
    [Min(-1)]
    public int SecondsUntilAssist = -1;
    public Vector3 compassPosOffset; // where is the compass relative to the active player snappoint
    public Vector3 compassRotOffset; // compass rotation relative to the active player snap point

    // For logging output
    private float startTime;
    private Vector3 playerLastPosition;
    private float playerDistance = 0;
    private Vector3 scaledPlayerLastPosition;
    private float scaledPlayerDistance = 0;
    private List<float> distances = new List<float>();

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
		hud.showScore = showScoring;


        // Disable the objects to start
        targets = GameObject.FindGameObjectsWithTag("Target");

        foreach (GameObject tar in targets) {
            tar.SetActive(false);
        }


        // Change text and turn on the map action button
        if (shortcut) {
            // actionButton.GetComponentInChildren<Text>().text = buttonText;
		    // hud.actionButton.SetActive(true);
		    // hud.actionButton.GetComponent<Button>().onClick.AddListener(hud.OnActionClick);
            // make the cursor functional and visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // if it's a target, open the door to show it's active
        // if this is NavRDJ, only open certain doors
        if (!shortcut) {
            currentDoor = Doors.currentString();
            Debug.Log(currentDoor);

            doorList = currentDoor.Split(new char[] {','});

            foreach (string doorName in doorList) {
                if (doorName == "") {
                    break;
                }
                door = GameObject.Find(doorName);
                // Debug.Log("found door");
                door.GetComponentInChildren<AKB_Door>().OpenDoor();
            }
        } else {    // otherwise open all the doors
            doors = GameObject.FindGameObjectsWithTag("Doors");

            foreach (GameObject door in doors) {
                door.GetComponentInChildren<AKB_Door>().doorMaxOpenAngle = -20;
                door.GetComponentInChildren<AKB_Door>().OpenDoor();
            }
        }

        foreach (GameObject tar in targets) {
            tar.SetActive(true);
        }
		
        // Parse the path we are supposed to follow
        currentPath = Rooms.currentString();
        Debug.Log(currentPath);

        roomList = currentPath.Split(new char[] {','});
        roomObjList = new List<string>();
        foreach (string roomName in roomList) {
            if (string.IsNullOrWhiteSpace(roomName)) {
                break;
            } else {
                roomObjList.Add(roomName);
            }
            
        }

        objDict = objsAndLocs.objDict;
        centerDict = objsAndLocs.centerDict;
        start_destination = centerDict[GameObject.Find(roomObjList[0])];
        Debug.Log(start_destination.transform.position.ToString());
        // Debug.Log(GameObject.Find("obj20"));
        string endLocation = roomObjList.Last();
        endObject = objDict[GameObject.Find(endLocation)];
        Debug.Log(endObject);

        // Handle if we're hiding all the non-targets
        if (hideNonTargets)
        {
            startCue = objDict[GameObject.Find(roomObjList[0])];
            foreach (GameObject tar in targets) {
                tar.SetActive(false);
            }

            endObject.SetActive(true);
            // but turn on the first object
            startCue.SetActive(true);
        }
        // move person to start location
        if (scaledPlayer)
        {
            start = scaledAvatar;
        } else start = avatar;

        Debug.Log("Starting location selected: " + start_destination.name.ToString() +
                " (" + start_destination.transform.position.x + ", " +
                start_destination.transform.position.z + ")");

        position = start.transform.position;
        rotation = start.transform.eulerAngles;


        // Change the position (2D or 3D)
        start.GetComponentInChildren<CharacterController>().enabled = false;
        Vector3 tempPos = start.transform.position;
        tempPos.x = start_destination.transform.position.x;
        if (!ignoreY)
        {
            tempPos.y = start_destination.transform.position.y;
        }
        tempPos.z = start_destination.transform.position.z;
        start.transform.position = tempPos;
        log.log("TASK_POSITION\t" + start.name + "\t" + this.GetType().Name + "\t" + start.transform.transform.position.ToString("f1"), 1);
        Debug.Log("Player now at: " + start.name +
                " (" + start.transform.position.x + ", " +
                start.transform.position.z + ")");
        start.GetComponentInChildren<CharacterController>().enabled = true;

        // Set the rotation to random if selected
        if (randomRotation)
        {
        Vector3 tempRot = start.transform.eulerAngles;
        tempRot.y = Random.Range(0, 359.999f);
        start.transform.eulerAngles = tempRot;
        }

        // Debug.Log ("Find " + endObject.name);
        

        // AKB - come back and change to general instruction "follow the path, keeping track of 
        // objects you see along the way
		if (NavigationInstruction)
		{
			string msg = NavigationInstruction.text;
			if (roomObjList != null) msg = string.Format(msg);
			hud.setMessage(msg);
   		}
		else
		{
            hud.SecondsToShow = 0;
            // hud.setMessage("Please find the " + current.name);
		}

        


        // Handle if we're hiding the target object
        if (hideTargetOnStart != HideTargetOnStart.Off)
        {
            if (hideTargetOnStart == HideTargetOnStart.SetInactive)
            {
                endObject.SetActive(false);
            }
            else if (hideTargetOnStart == HideTargetOnStart.SetInvisible)
            {
                endObject.GetComponent<MeshRenderer>().enabled = false;
            }
            else if (hideTargetOnStart == HideTargetOnStart.SetProbeTrial)
            {
                endObject.SetActive(false);
                endObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            endObject.SetActive(true); // make sure the target is visible unless the bool to hide was checked
            try
            {
                endObject.GetComponent<MeshRenderer>().enabled = true;
            }
            catch (System.Exception ex)
            {

            }
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
        // AKB - change to calculate distances between starting location and two target objects
        // if (isScaled)
        // {
        //     foreach (GameObject target in targetObjects.objects) {
        //         distances.Add(Vector3.Distance(scaledAvatar.transform.position, target.transform.position));
        //     }
            
        // }
        // else 
        // {
        //     foreach (GameObject target in targetObjects.objects) {
        //         distances.Add(Vector3.Distance(avatar.transform.position, target.transform.position));
        //     }
        // }


        // Grab our LM_Compass object and move it to the player snapPoint
        // if (assistCompass != null)
        // {
        //     assistCompass.transform.parent = avatar.GetComponentInChildren<LM_SnapPoint>().transform;
        //     assistCompass.transform.localPosition = compassPosOffset;
        //     assistCompass.transform.localEulerAngles = compassRotOffset;
        //     assistCompass.gameObject.SetActive(false);
        // }

        //// MJS 2019 - Move HUD to top left corner
        //hud.hudPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        //hud.hudPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.9f);
    }

    public override bool updateTask ()
	{
		base.updateTask();


        if (skip)
        {
            //log.log("INFO    skip task    " + name,1 );
            return true;
        }

        if (score > 0) penaltyTimer = penaltyTimer + (Time.deltaTime * 1000);

        // AKB question for DJH: should I have a penalty timer?
		if (penaltyTimer >= penaltyRate)
		{
			penaltyTimer = penaltyTimer - penaltyRate;
			if (score > 0)
			{
				score = score - 1;
				hud.setScore(score);
			}
		}
    

        // Keep updating the distance traveled
        playerDistance += Vector3.Distance(avatar.transform.position, playerLastPosition);
        playerLastPosition = avatar.transform.position;
        if (isScaled)
        {
            scaledPlayerDistance += Vector3.Distance(scaledAvatar.transform.position, scaledPlayerLastPosition);
            scaledPlayerLastPosition = scaledAvatar.transform.position;
        }


        // if player presses enter during shortcut round, end the trial
        // if (shortcut) {
        //     if (Input.GetKeyDown (KeyCode.Return))
        //     {
        //         // log.log("Selected :\t" + "SAME DISTANCE", 1);
        //         return true;
        //     }
        // }



		if (killCurrent == true)
		{
			return KillCurrent ();
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

        // otherwise close all the doors
        doors = GameObject.FindGameObjectsWithTag("Doors");

        foreach (GameObject door in doors) {
            door.GetComponentInChildren<AKB_Door>().doorMaxOpenAngle = -115;
            door.GetComponentInChildren<AKB_Door>().CloseDoor();
        }

        foreach (GameObject tar in targets) {
            tar.SetActive(true);
        }
    

        Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false;

        // turn off the map action button
		hud.actionButton.GetComponent<Button>().onClick.RemoveListener(hud.OnActionClick);
		actionButton.GetComponentInChildren<Text>().text = actionButton.GetComponent<DefaultText>().defaultText;
		hud.actionButton.SetActive(false);


        // increment everything
        if (Doors) Doors.incrementCurrent();
        if (shortcut) {
            Rooms.incrementCurrent();
            // startCue.SetActive(false);
        }


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

        // AKB COME BACK !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        var parent = this.parentTask;
        var masterTask = parent;
        // while (!masterTask.gameObject.CompareTag("Task")) masterTask = masterTask.parentTask;
        // This will log all final trial info in tab delimited format
        // var excessPath = perfDistance - optimalDistance;

        var navTime = Time.time - startTime;

        // set impossible values if the nav task was skipped
        if (skip)
        {
            navTime = float.NaN;
            perfDistance = float.NaN;
            distances.Add(float.NaN);
            distances.Add(float.NaN);
            // excessPath = float.NaN;
        }
        

        log.log("LM_OUTPUT\tNavigationTask.cs\t" + masterTask + "\t" + this.name + "\n" +
        	"Task\tBlock\tTrial\tTargetName\tActualPath\tRouteDuration\n" +
        	masterTask.name + "\t" + masterTask.repeatCount + "\t" + parent.repeatCount + "\t" + Rooms.currentString() + "\t"+ perfDistance + "\t" + navTime
            , 1);


        // More concise LM_TrialLog logging
        if (trialLog.active)
        {
            trialLog.AddData(transform.name + "_target", Rooms.currentString());
            trialLog.AddData(transform.name + "_actualPath", perfDistance.ToString());
            // trialLog.AddData(transform.name + "_distances", distances.ToString());
            // trialLog.AddData(transform.name + "_excessPath", excessPath.ToString());
            trialLog.AddData(transform.name + "_duration", navTime.ToString());
        }
    }

	public override bool OnControllerColliderHit(GameObject hit)
	{
		if (hit == endObject)
		{
			if (showScoring)
			{
				score = score + scoreIncrement;
				hud.setScore(score);
			}
			return true;
		}

		//		Debug.Log (hit.transform.parent.name + " = " + current.name);
		if (hit.transform.parent == endObject.transform)
		{
			if (showScoring)
			{
				score = score + scoreIncrement;
				hud.setScore(score);
			}
			return true;
		}
		return false;
	}
}