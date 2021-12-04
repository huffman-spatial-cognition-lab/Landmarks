/*
    This class will destroy child game objects (e.g., to clean up copies).

    Initial starting point:
        MapTestTask.cs (by Michael J. Starrett and Jared D. Stokes)

    Written by Derek J. Huffman (Summer 2021).
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DestroyChildObjects : ExperimentTask
{

	[Header("Task-specific Properties")]

	public GameObject copyObjects;

	public override void startTask()
	{
		TASK_START();
		Debug.Log("task is starting");
		avatarLog.navLog = false;
	}


	public override void TASK_START()
	{
		if (!manager) Start();
		base.startTask();

	}


	public override bool updateTask()
	{
		base.updateTask();

		return true;
	}



	public override void endTask()
	{
		TASK_END();
	}


	public override void TASK_END()
	{
		base.endTask();

		Debug.Log("HERE WE ARE AGAIN!!!!");

		// Destroy the copies we created when initializing the map test task
		foreach (Transform child in copyObjects.transform)
		{
			Destroy(child.gameObject);
		}

	}

}