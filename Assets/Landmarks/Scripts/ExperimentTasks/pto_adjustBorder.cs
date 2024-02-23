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
                Debug.Log("We made it to the get component enablethenfade in bit");
                child.gameObject.GetComponent<fadeable>().EnableThenFadeIn();
            }
            else
            {
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
    }

}