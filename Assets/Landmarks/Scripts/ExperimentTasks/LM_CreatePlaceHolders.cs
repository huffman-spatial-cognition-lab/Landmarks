using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_CreatePlaceHolders : ExperimentTask
{


    [Header("Task-specific Properties")]

    public ObjectList targetObjectList;
    public float placeholderSpacing = 10.0f;

    // DJH - adding functionality for offset
    public float offsetX = 985.0f;
    public float offsetZ = 1025.0f;

    public float offsetX2 = 990.0f;
    public float offsetZ2 = 1050.0f;

    public float offsetX3 = 995.0f;
    public float offsetZ3 = 1075.0f;


    private GameObject placeholder;

    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        positionPlaceholder();


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

    public void positionPlaceholder()
  {
    //// only generate placeholders on the first run
    //if (parentTask.repeatCount > 1)
    //{
    //    skip = true;
    //}

    if (skip)
    {
        return;
    }

    for (int i = 0; i < targetObjectList.objects.Count; i++)
    {
        placeholder = new GameObject("PlaceHolder");
        placeholder.transform.parent = transform;
        // DJH - Adding functionality for offset
        // Prev:
        //placeholder.transform.localPosition = new Vector3(0.0f, 0.0f, -1 * i * placeholderSpacing);
        // Updated:
        if (i <= 8)
        {
          placeholder.transform.localPosition = new Vector3(offsetX, 1.5f, offsetZ + (-1 * i * placeholderSpacing));

        }
        else if (i > 8 && i <= 17)
        {
          placeholder.transform.localPosition = new Vector3(offsetX2, 1.5f, offsetZ2 + (-1 * i * placeholderSpacing));

        }
        else if (i > 17)
        {
            placeholder.transform.localPosition = new Vector3(offsetX3, 1.5f, offsetZ3 + (-1 * i * placeholderSpacing));

        }

        }
  }



}
