/*    
    Sinan Yumurtaci -- Colby College
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pto_trialsTruth : ExperimentTask
{
    [Header("Task-specific Properties")]

    public AllTrials allTrials;
    int currentParticipantNo;
    public int participantNumOffset = 101;
    public Trials trialsTruth;
    public string json_trials_file = "json/dynamic";

    public override void startTask()
    {
        TASK_START();

    }
    
    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // WRITE TASK STARTUP CODE HERE
        allTrials = ImportFromJSON(json_trials_file);
        currentParticipantNo = manager.config.id;
        Debug.Log("Current No.\t" + currentParticipantNo + "\tOffset:\t" + participantNumOffset);
        Debug.Log("Equals\t" + (currentParticipantNo - participantNumOffset).ToString());
        trialsTruth = allTrials.allTrials[currentParticipantNo - participantNumOffset];
        Debug.Log("imported trials data from JSON!");
        Debug.Log(trialsTruth);
    }

    public static AllTrials ImportFromJSON(string path)
    {
        TextAsset jsonStringAsset = Resources.Load<TextAsset>(path);
        string jsonString = jsonStringAsset.text;

        return JsonUtility.FromJson<AllTrials>(jsonString);
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


[Serializable]
public class AllTrials
{
public List<Trials> allTrials;
}

[Serializable]
public class Trials
{
public List<Trial> trials;
}

[Serializable]
public class Trial
{
public bool boundaryVisible;
public List<TargetObject> targetObjects;
}

[Serializable]
public class TargetObject
{
public float x;
public float y;
public string object_repr;
}