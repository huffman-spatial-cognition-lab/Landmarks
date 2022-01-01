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

    public TrialsData trialsTruth;

    public override void startTask()
    {
        TASK_START();

    }
    
    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // WRITE TASK STARTUP CODE HERE
        trialsTruth = ImportFromJSON("json/test");
        Debug.Log("imported trials data from JSON!");
        Debug.Log(trialsTruth);
    }

    public static TrialsData ImportFromJSON(string path)
    {
        TextAsset jsonStringAsset = Resources.Load<TextAsset>(path);
        string jsonString = jsonStringAsset.text;

        return JsonUtility.FromJson<TrialsData>(jsonString);
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
public class TrialsData
{
public List<Trial> trials;
}

[Serializable]
public class Trial
{
public bool boundaryVisible;
public List<ExpObject> expObjects;
}

[Serializable]
public class ExpObject
{
public float x;
public float y;
}
