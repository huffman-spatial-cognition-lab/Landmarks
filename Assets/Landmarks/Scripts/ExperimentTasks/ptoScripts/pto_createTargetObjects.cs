/*    
    Sinan Yumurtaci -- Colby College
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pto_createTargetObjects : ExperimentTask
{
    private List<string> OBJ_COLOR_NAMES = new List<string>() {"red", "orange", "yellow", "green", "cyan", "purple", "white", "blue"};
    private List<string> OBJ_SHAPE_NAMES = new List<string>() {"capsule", "cube", "cylinder", "sphere"};

    [Header("Task-specific Properties")]
    public Transform TargetObjectTemplatesParent;

    public List<Material> OBJ_COLOR_MATERIALS;
    public List<GameObject> OBJ_SHAPES;

    public override void startTask()
    {
        TASK_START();

    }
    
    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        for (int colori = 0; colori < OBJ_COLOR_NAMES.Count; colori += 1){
            for (int shapei = 0; shapei < OBJ_SHAPE_NAMES.Count; shapei += 1){
                GameObject targetObject = Instantiate(OBJ_SHAPES[shapei], TargetObjectTemplatesParent);
                string targetObjectName = OBJ_COLOR_NAMES[colori] + "_" + OBJ_SHAPE_NAMES[shapei];

                targetObject.GetComponent<Renderer>().material = OBJ_COLOR_MATERIALS[colori];
                targetObject.AddComponent<targetObjectScript>();
                targetObject.AddComponent<fadeable>();
                targetObject.name = targetObjectName;
            }
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
    }

}