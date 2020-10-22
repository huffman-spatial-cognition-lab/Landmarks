/*
    LM Dummy
       
    Attached object holds task components that need to be effectively ignored 
    by Tasklist but are required for the script. Thus the object this is 
    attached to can be detected by Tasklist (won't throw error), but does nothing 
    except start and end.   

    Copyright (C) 2019 Michael J. Starrett

    Navigate by StarrLite (Powered by LandMarks)
    Human Spatial Cognition Laboratory
    Department of Psychology - University of Arizona   
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class LM_ObjectPlacement : ExperimentTask
{
    [Header("Task-specific Properties")]
    public GameObject markerObjectTemplate;
    [Range(0.0f, 100.0f)]
    public float markerStartDistance;
    public float translationSpeed;

    // Private Variables
    
    private GameObject markerObject;
    private Vector3 markerLocation;
    private bool oriented = false;

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
        hud.showEverything();
        
    }


    public override bool updateTask()
    {

        // WRITE TASK UPDATE CODE HERE

        // During the orienting stage
        if ( Input.GetKeyDown(KeyCode.Return) && !oriented)
        {
            if (vrEnabled)
            {

            }
            else 
            {
                // Lock player movment & reset looking
                avatar.GetComponent<FirstPersonController>().enabled = false; // disable the controller to work
                avatar.GetComponentInChildren<Camera>().transform.localEulerAngles = Vector3.zero; // reset the camera
                avatar.GetComponent<FirstPersonController>().ResetMouselook(); // reset the zero position to be our current cam orientation
            }

            // Instantiate the Marker
            markerLocation = manager.player.transform.position; //TODO player location + orientation * 1
            markerObject = Instantiate(markerObjectTemplate, markerLocation, Quaternion.identity);
            markerObject.transform.localPosition = new Vector3(0,0, -markerStartDistance);
            markerObject.transform.localEulerAngles = Vector3.zero;

            // label them as oriented
            oriented = true;
        }

        // check for key updates
        // and move markerLocation accordingly
        // and update the markerObject.Transform

        if (vrEnabled)
        {
            Debug.Log("todo");
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow)){

            }
            if (Input.GetKeyDown(KeyCode.RightArrow)){

            }
            if (Input.GetKeyDown(KeyCode.UpArrow)){

            }
            if (Input.GetKeyDown(KeyCode.DownArrow)){

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

        // WRITE TASK EXIT CODE HERE

        //TODO: remove marker object
    }

}