/*
    This class will gather the transform of the door in its initial state.
    The idea is to use this to reset the door transform on each trial, which
    helps fix the bug where the door wants to close to wherever its initial transform was.
    Basically, if the door was turned off when it was slightly opened (e.g., from previous
    trial), then it would try to "close" to that initial position. The idea here is to
    gather the actual starting location at the beginning of the task to reset to this
    position for each time we use pto_adjustBorder script on the door object. This
    will also fix the bug where if we only change the rotation, then the position can
    be slightly off.
    
    Written by Derek J. Huffman (2024 March 06)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gatherDoorInitTransform : ExperimentTask
{
    [Header("Task-specific Properties")]
    public GameObject swingingDoor;
    public Vector3 doorInitTransform;
    public Quaternion doorInitRotation;

    public Vector3 doorHandleInitPos;
    public Quaternion doorHandleInitRotation;

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // gather the initial transform and rotation information
        doorInitTransform = swingingDoor.transform.localPosition;
        doorInitRotation = swingingDoor.transform.localRotation;

        // gather the initial transform and rotation information for the door handle
        foreach (Transform child in swingingDoor.transform)
        {
            doorHandleInitPos = child.gameObject.transform.localPosition;
            doorHandleInitRotation = child.gameObject.transform.localRotation;
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