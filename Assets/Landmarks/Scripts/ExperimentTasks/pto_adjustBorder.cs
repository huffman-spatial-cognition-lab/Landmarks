/*
    pto_adjustBorder
    ``
    toggles the border on conditionally, or off
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum adjustState{
    on,
    onCondition,
    off
}
public class pto_adjustBorder : ExperimentTask
{
    [Header("Task-specific Properties")]
    public adjustState mode;
    public GameObject borderParent;
    public Material materialOpaque;
    public Material materialFade;

    public bool isSwingingDoor = false;

    private GameObject trialData; // game object from which to pull the masterTrialMatrix data

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }

    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        bool newBorderActive = false;

        trialData = GameObject.Find("TrialsTruth");

        if(mode == adjustState.onCondition){
            int trialIndex = this.parentTask.repeatCount;
            // repeatCount starts from 1, we subtract one to 0-index into the arrays
            bool borderIsOn = trialData.GetComponent<pto_trialsTruth>().trialsTruth.trials[this.parentTask.repeatCount - 1].boundaryVisible;

            Debug.Log("THE CURRENT STATE OF borderIsOn");
            Debug.Log(borderIsOn);
            
            if(borderIsOn){
                newBorderActive = true;
            }else{
                newBorderActive = false;
            }
            
        }else if(mode == adjustState.off){
            newBorderActive = false;
        }else if(mode == adjustState.on){
            newBorderActive = true;
        }

        Debug.Log("THE CURRENT STATE OF newBorderActive");
        Debug.Log(newBorderActive);

        foreach (Transform child in borderParent.transform) {
            if(newBorderActive)
            {
                // set the door to a default of being closed (here z = 0), when it loads
                // to fix bug where it won't close if it was inactivated in an open position
                // i.e., the door seems to set its zero point based on the angle it was instantiated
                if (isSwingingDoor)
                {
                    // get the initial transform local position and local rotation
                    Vector3 initLocalPosition = GameObject.Find("GatherInitialDoorTransform").GetComponent<gatherDoorInitTransform>().doorInitTransform;
                    Quaternion initLocalRotation = GameObject.Find("GatherInitialDoorTransform").GetComponent<gatherDoorInitTransform>().doorInitRotation;
                    Debug.Log("-------------------------------");
                    Debug.Log(initLocalPosition);
                    Debug.Log(initLocalRotation);
                    // gather the game object so that we can reset its transform to the original local position and rotation
                    GameObject currentObject = child.gameObject;
                    // set the local position
                    currentObject.transform.localPosition = initLocalPosition;
                    // set the local rotation
                    currentObject.transform.localRotation = initLocalRotation;
                }
                Debug.Log("We made it to the get component enablethenfade in bit");
                child.gameObject.GetComponent<fadeable>().EnableThenFadeIn(materialFade, materialOpaque);
            }
            else
            {
                Debug.Log("We made it to the get component FADEOUTTHENDISABLE");
                child.gameObject.GetComponent<fadeable>().FadeOutThenDisable(materialFade, materialOpaque);
            }
        }

    }


    public override bool updateTask()
    {
        return true;
        // WRITE TASK UPDATE CODE HERE
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
        foreach (Transform child in borderParent.transform)
        {
            // Save whether each is active/inactive to the log file
            log.log("pto_adjustBorder.cs\t" + this.parentTask.repeatCount + "\tNAME\t" + child.transform.gameObject.name + "\tactiveSELF\t" + child.transform.gameObject.activeSelf, 1);
            if (child.transform.gameObject.activeSelf)
            {
                Debug.Log("We made it to the ENDTASK as active");
                //child.gameObject.GetComponent<fadeable>().EnableThenFadeIn();
            }
            else
            {
                Debug.Log("We made it to the ENDTASK as inactive");
                //child.gameObject.GetComponent<fadeable>().FadeOutThenDisable(materialFade, materialOpaque);
            }
        }
    }

}