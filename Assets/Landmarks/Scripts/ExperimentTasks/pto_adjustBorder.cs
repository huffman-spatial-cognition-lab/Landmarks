/*
    pto_adjustBorder
    ``
    toggles the border on conditionally, or off
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum adjustState{
    onCondition,
    off
}
public class pto_adjustBorder : ExperimentTask
{
    [Header("Task-specific Properties")]
    public adjustState mode;
    public GameObject borderParent;

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
            bool borderIsOn = trialData.GetComponent<pto_trialsTruth>().trialsTruth.trials[this.parentTask.repeatCount].boundaryVisible;
            
            if(borderIsOn){
                newBorderActive = true;
            }else{
                newBorderActive = false;
            }
            
        }else if(mode == adjustState.off){
            newBorderActive = false;
        }

        foreach (Transform child in borderParent.transform) {
            child.gameObject.SetActive(newBorderActive);
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