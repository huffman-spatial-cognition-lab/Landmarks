/*
    ToggleObjects

    Toggles all objects in the given list.

    Sinan Yumurtaci -- Colby College, ME
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjects : ExperimentTask
{
    [Header("Task-specific Properties")]
    public List<GameObject> parentObjects;

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        foreach(GameObject parentObject in parentObjects){

            foreach (Transform child in parentObject.transform) {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
                Debug.Log("1");
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