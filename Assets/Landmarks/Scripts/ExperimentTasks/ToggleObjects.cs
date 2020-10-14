/*
    ToggleObjects

    Toggles all objects in the given list.

    Sinan Yumurtaci -- Colby College, ME
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ToggleMode
{
    setTrue, // true
    setFalse, // false
    toggle // ! of current state
}

public class ToggleObjects : ExperimentTask
{
    [Header("Task-specific Properties")]
    public List<GameObject> parentObjects;
    public ToggleMode mode;

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
        bool newVal;
            foreach (Transform child in parentObject.transform) {
                switch (mode)
                {
                    case ToggleMode.setTrue:
                        newVal = true;
                        break;
                    case ToggleMode.setFalse:
                        newVal = false;
                        break;
                    case ToggleMode.toggle:
                        newVal = !child.gameObject.activeSelf;
                        break;
                    default:
                        newVal = !child.gameObject.activeSelf;
                        break;
                }
                child.gameObject.SetActive(newVal);
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