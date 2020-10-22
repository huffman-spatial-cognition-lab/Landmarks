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
    public GameObject trialData; // game object from which to pull the masterTrialMatrix data

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        Debug.Log(trialData.GetComponent<pto_generateStartGoalPairs>().masterTrialMatrix);

        if(mode == adjustState.onCondition){

        }else if(mode == adjustState.off){

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