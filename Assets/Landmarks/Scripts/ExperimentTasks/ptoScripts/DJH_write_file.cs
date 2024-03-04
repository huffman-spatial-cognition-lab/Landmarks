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

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJH_write_file : ExperimentTask
{
    [Header("Task-specific Properties")]
    public GameObject dummyProperty;

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
        string dataPath = Application.persistentDataPath;
        string filename = "testing.txt";
        string full_filename = Path.Combine(dataPath, filename);
        Debug.Log("HERE WE ARE!!! HERE'S WHERE WE'RE WRITING THE FILE:");
        Debug.Log(full_filename);
        StreamWriter sw = new StreamWriter(full_filename);
        sw.WriteLine("Here we are!!!");
        sw.Close();
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