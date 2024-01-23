using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyObjectsAB : ExperimentTask {

    [Header("Task-specific Properties")]

	public AB_CreatePlaceHolders Placeholders; 
	private List<GameObject> destinationsParent;
    public ExperimentTask Objs;
	public string currentTrial;
    private string[] objList;
	public MoveObjects objsAndLocs;
    private Dictionary<GameObject,GameObject> objDict;
	private GameObject target1;
	private GameObject target2;

	
	private List<GameObject> sourcesParent;
    public List<GameObject> copies;

	public bool setOriginalInactive = true;
    public bool randomlyRotateCopy = false;

	private string copiedParent;

	public override void startTask ()
	{
		TASK_START ();
	}

	public override void TASK_START () 
	{
		base.startTask ();

        copies = new List<GameObject>();

        // Grab target objects
        currentTrial = Objs.currentString();

        objList = currentTrial.Split(new char[] {','});

		GameObject loc1 = GameObject.Find(objList[0]);
		GameObject loc2 = GameObject.Find(objList[1]);

		objDict = objsAndLocs.objDict;
		target1 = objDict[loc1];
		target2 = objDict[loc2];

		sourcesParent = new List<GameObject>();

        sourcesParent.Add(target1);
        sourcesParent.Add(target2);

		destinationsParent = Placeholders.destinations;


		// move the copy destination parent to the same place as the sourcesParent to be copied
		// this.transform.position = destinationsParent.gameObject.transform.position;
		// this.transform.rotation = destinationsParent.gameObject.transform.rotation;

		for (int i = 0; i < sourcesParent.Count; i++)
		{	
			Debug.Log(i);
            GameObject sourceChild = sourcesParent[i];
			Debug.Log("sourceParent not empty");
            GameObject destinationChild = destinationsParent[i];
			Debug.Log("destinationParent not empty");

			GameObject copy = Instantiate<GameObject> (sourceChild, destinationChild.transform.position, sourceChild.transform.rotation, this.transform);

            if (randomlyRotateCopy)
            {
                // Randomly rotate the copied object 0, 90, 180, or 270 degrees
                List<float> rotateOptions = new List<float> { 0.0f, 90.0f, 180.0f, 270.0f };
                copy.transform.localEulerAngles = new Vector3(copy.transform.localEulerAngles.x, rotateOptions[Random.Range(0, rotateOptions.Count)], copy.transform.localEulerAngles.z);
            }
			else
            {
				// set local rotation to zero
				copy.transform.localEulerAngles = new Vector3(copy.transform.localEulerAngles.x, 0f, copy.transform.localEulerAngles.z);
			}
           
            copy.name = sourceChild.name;

            copies.Add(copy);
			Debug.Log(copy.transform.position);

            // if (setOriginalInactive == true) {
			// 	sourcesParent[i].SetActive(false);
			// }
		}

	}

	public override bool updateTask ()
	{
		return true;
	}

	public override void endTask() {
		TASK_END();
	}

	public override void TASK_END() {
		base.endTask();
	}
}