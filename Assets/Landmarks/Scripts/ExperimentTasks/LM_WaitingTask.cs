/*
    Derek J. Huffman  
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_WaitingTask : ExperimentTask
{
    [Header("Task-specific Properties")]
    public int timeToWait = 1000;
    private long timeStarted;

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
        timeStarted = Experiment.Now();
        Debug.Log("CURRENT TIME:");
        Debug.Log(timeStarted);
        Debug.Log("TIME STARTED + TIME TO WAIT");
        Debug.Log(timeStarted + timeToWait);
    }


    public override bool updateTask()
    {
        // WRITE TASK UPDATE CODE HERE
        if (Experiment.Now() >= (timeStarted + timeToWait)) {
            Debug.Log("WE ARE OUTTA HERE!!!");
            return true;
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
    }

}