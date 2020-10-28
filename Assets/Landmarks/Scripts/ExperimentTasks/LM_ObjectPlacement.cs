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
    [Range(-1.0f, 3.0f)]
    public float markerFixedHeight;
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
                //avatar.GetComponentInChildren<Camera>().transform.localEulerAngles = new Vector3 (25, 0, 0); // reset the camera
                avatar.GetComponent<FirstPersonController>().ResetMouselook(); // reset the zero position to be our current cam orientation
            }

            // Instantiate the Marker
            markerLocation = manager.player.transform.position + avatar.GetComponentInChildren<Camera>().transform.forward * markerStartDistance;
            markerLocation.y = markerFixedHeight;
            markerObject = Instantiate(markerObjectTemplate, markerLocation, Quaternion.identity);
            //markerObject.transform.localPosition = new Vector3(0,0, -markerStartDistance);    
            //markerObject.transform.localEulerAngles = Vector3.zero;

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
            markerLocation += targetTranslation;
            markerLocation.y = markerFixedHeight;
            markerObject.transform.position = markerLocation;
            

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