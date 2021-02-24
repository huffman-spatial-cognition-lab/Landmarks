using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

enum PointingTaskStage{
    Orienting,
    Pointing
}

public class LM_ObjectPlacement : ExperimentTask
{
    [Header("Task-specific Properties")]
    public GameObject markerObjectTemplate;
    public LineRenderer lineRendererTemplate;
    [Range(0.0f, 100.0f)]
    public float markerStartDistance;
    [Range(-1.0f, 3.0f)]
    public float markerFixedHeight;
    public float translationSpeed;

    public Transform RightHand;

    // Private Variables
    
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
        
        lineRendererTemplate.gameObject.SetActive(false);
		_lineRenderer = Instantiate(lineRendererTemplate);

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
                    Debug.Log("todo--objectPlacement--VR");
                }
                else 
                {
                    // Lock player movement & reset looking at the current orientation ONLY IF KEYBOARD CONTROLS
                    avatar.GetComponent<FirstPersonController>().enabled = false; // disable the controller to work
                }

                // Instantiate the Marker
                markerLocation = manager.player.transform.position + avatar.GetComponentInChildren<Camera>().transform.forward * markerStartDistance;
                markerLocation.y = markerFixedHeight;
                markerObject = Instantiate(markerObjectTemplate, markerLocation, Quaternion.identity);
                Debug.Log(markerLocation);
                //markerObject.transform.localPosition = new Vector3(0,0, -markerStartDistance);    
                //markerObject.transform.localEulerAngles = Vector3.zero;

                // move on to the next stage
                stage = PointingTaskStage.Pointing;
            }
        }

        // During the pointing stage (after orienting)
        if (stage == PointingTaskStage.Pointing){

            // check for key updates
            // and move markerLocation accordingly
            // and update the markerObject.Transform

            if (vrEnabled)
            {
                int range = 100;
                bool aimHit = false;
		        Ray aimRay = new Ray(RightHand.position, RightHand.forward);
                Vector3 startPoint = aimRay.origin;
                Vector3 endPoint = aimRay.origin + aimRay.direction * range;
                RaycastHit hitInfo;

                Debug.Log(startPoint);

                if(Physics.Raycast(startPoint, aimRay.direction, out hitInfo, range)){//, out hitInfo, range, QueryTriggerInteraction.Ignore)){
                    endPoint = startPoint + aimRay.direction * hitInfo.distance;
                    aimHit = true;
                    Debug.Log(hitInfo);
                    Debug.Log(hitInfo.point);
                    Debug.Log(hitInfo.collider);
                }

                markerObject.transform.position = hitInfo.point;

                // TODO update the line renderer properties based on hit data
                _lineRenderer.gameObject.SetActive(true);
                _lineRenderer.sharedMaterial.color = Color.green;
                _lineRenderer.positionCount = 2;
                _lineRenderer.SetPosition(0, startPoint);
                _lineRenderer.SetPosition(0, endPoint);
                

               
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
            // todo: how to "Return" with the VR?
            if ( Input.GetKeyDown(KeyCode.Return))
                {
                    Debug.Log("todo--objectPointing--logging");
                    return true;
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

        if (trialLog.active)
        {
            trialLog.AddData(transform.name + "_task", "ObjectPlacement");
            /*
            trialLog.AddData(transform.name + "_playerLocation", startAngle.ToString()); // record where we started the compass at
            trialLog.AddData(transform.name + "_responseCW", response.ToString());
            trialLog.AddData(transform.name + "_answerCW", answer.ToString());
            trialLog.AddData(transform.name + "_signedError", signedError.ToString());
            trialLog.AddData(transform.name + "_absError", absError.ToString());
            trialLog.AddData(transform.name + "_SOPorientingTime", orientTime.ToString());
            trialLog.AddData(transform.name + "_responseTime", responseTime.ToString());
            */
        }

        Destroy(markerObject);

        if(!vrEnabled){
            avatar.GetComponent<FirstPersonController>().enabled = true;
        }

        stage = PointingTaskStage.Orienting; // return to the original stage
    }

}