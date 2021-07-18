﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_CreatePlaceHolders : ExperimentTask
{


    [Header("Task-specific Properties")]

    public ObjectList targetObjectList;
    public float placeholderSpacing = 10.0f;

    // DJH - adding functionality for offset
    public string overheadCameraName = "OverheadCamera";

    private GameObject placeholder;

    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        //// only generate placeholders on the first run
        //if (parentTask.repeatCount > 1)
        //{
        //    skip = true;
        //}

        if (skip)
        {
            return;
        }

        // DJH - Adding functionality for offset
        Vector3 overheadCameraTransform = GameObject.Find(overheadCameraName).transform.position;
        float offsetX = overheadCameraTransform.x;
        float offsetZ = overheadCameraTransform.z;

        for (int i = 0; i < targetObjectList.objects.Count; i++)
        {
            placeholder = new GameObject("PlaceHolder");
            placeholder.transform.parent = transform;
            // DJH - Adding functionality for offset
            // Prev:
            //placeholder.transform.localPosition = new Vector3(0.0f, 0.0f, -1 * i * placeholderSpacing);
            // Updated: 
            placeholder.transform.localPosition = new Vector3(offsetX, 0.0f, offsetZ + (-1 * i * placeholderSpacing));
        }
    }


    public override bool updateTask()
    {
        return true;
    }


    public override void endTask()
    {
        TASK_END();
    }


    public override void TASK_END()
    {
        base.endTask();

        skip = true; // don't run a second time (creates extra placeholders)
    }

}
