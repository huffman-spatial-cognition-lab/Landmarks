/*
    Copyright (C) 2010  Jason Laczko

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

    # -------------------------------------------------------------------------
    This script was modified from the original ViewTargets.cs script. Here, I
    am writing a script that will run with a more standard lab-based task of
    showing objects with set timing and randomization.

    WRITTEN BY DEREK J. HUFFMAN (JANUARY 2022).
    # -------------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Characters.FirstPerson;
using System.Runtime.InteropServices;
using LSL;


public class ViewStimuliTimed : ExperimentTask {

    [Header("Task-specific Properties")]

    public ObjectList startObjects;
	public LM_RandomOrderStimuli randomOrderStimuli;
	private GameObject current;
	[HideInInspector] public GameObject destination;

	private static Vector3 position;
	private static Quaternion rotation;
	private static Transform parent;
	private static Vector3 scale;
	private int saveLayer;
	private int viewLayer = 11;
	public bool blackout = true;
	public bool showName = false;
	private long trial_start;
	public Vector3 objectRotationOffset;
	public Vector3 objectPositionOffset;
    public bool restrictMovement = true;
	public bool eeg_lsl = true;
	public float sample;
	private bool first_frame = true;
	//public Vector3 size;



    private Vector3 initialHUDposition;

    public override void startTask () {
		TASK_START();
		current = startObjects.currentObject();

		initCurrent();
		trial_start = Experiment.Now();
		first_frame = true;
		// Debug.Log(trial_start);

	}



	public override void TASK_START()
	{


		if (!manager) Start();
		base.startTask();
		if (skip) {
			log.log("INFO	skip task	" + name,1 );
			return;
		}


	    if (blackout) hud.showOnlyTargets();
	    else hud.showEverything();

        if (restrictMovement)
        {
            manager.player.GetComponent<CharacterController>().enabled = false;
			if (avatar.GetComponent<FirstPersonController>())
			{
				avatar.GetComponentInChildren<Camera>().transform.localEulerAngles = Vector3.zero;
				avatar.GetComponent<FirstPersonController>().ResetMouselook();
				avatar.GetComponent<FirstPersonController>().enabled = false;
			}
			manager.scaledPlayer.GetComponent<ThirdPersonCharacter>().immobilized = true;
        }

		destination = avatar.GetComponentInChildren<LM_SnapPoint>().gameObject;

		// handle changes to the hud
		if (vrEnabled)
		{
			initialHUDposition = hud.hudPanel.transform.position;

			var temp = destination.transform.position;
			// If showName, then put in place, otherwise move under the floor (DJH)
			if (showName)
			{
				temp.y += 2.5f;
			}
			else
            {
				temp.y -= 100f;
            }
			hud.hudPanel.transform.position = temp;

		}
		else
		{
			// Change the anchor points to put the message at the bottom
			RectTransform hudposition = hud.hudPanel.GetComponent<RectTransform>() as RectTransform;
			// If showName, then put in place, otherwise move under the floor (DJH)
			if (showName)
			{
				hudposition.pivot = new Vector2(0.5f, 0.1f);
			}
			else
            {
				hudposition.pivot = new Vector2(0.5f, 100f);
			}
		}


        // turn off all objects
        foreach (GameObject item in startObjects.objects)
        {
            item.SetActive(false);
        }


    }



	public override bool updateTask () {

		if (skip) {
			//log.log("INFO	skip task	" + name,1 );
			return true;
		}

		// if we want to run LSL, then push to LSL here (DJH) -----------------
		if (eeg_lsl)
		{
			eeg_lsl_wait_for_end_of_frame();
		}


		if (current)
		{

			if (Experiment.Now() - trial_start >= interval)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return true;
		}

		return false;


	}


	public void eeg_lsl_wait_for_end_of_frame()
    {
		// for now, let's only run this on the first frame ------------------------
		if (first_frame)
		{
			// get the information ready for pushing the sample -------------------
			if (randomOrderStimuli.getUprightInverted() == 0)
			{
				sample = 360 + randomOrderStimuli.getCurrentGameObjectIndex();
			}
			else
			{
				sample = 180 + randomOrderStimuli.getCurrentGameObjectIndex();
			}
			Debug.Log(sample);
			initializeLSL.currentSample = new float[1] { sample };

			// wait for the end of frame ------------------------------------------
			wait_for_end_of_frame();

			// push the sample ----------------------------------------------------
			initializeLSL.lsl_push_sample();

			// reset trial_start here for more accurate timing --------------------
            trial_start = Experiment.Now();
			first_frame = false;
		}
	}

	// WaitForEndOfFrame for more accurate timing for LSL (DJH) ---------------
	IEnumerator wait_for_end_of_frame()
    {
		yield return new WaitForEndOfFrame();
	}


	public void initCurrent() {
		// store original properties of the target
		position = current.transform.position;
        rotation = current.transform.rotation;
		parent = current.transform.parent;
		scale = current.transform.localScale;



		//RectTransform rt = randomOrderStimuli.GetComponent<RectTransform>() as RectTransform;
		//float height = rt.rect.height;
		//size = renderer.bounds.size;

		// move the target to the viewing location temporarily
		current.transform.parent = destination.transform;
		current.transform.localPosition = objectPositionOffset;
		//current.transform.localPosition += new Vector3(0f, 0f, -0.5f * rt.rect.height);
        current.transform.localEulerAngles = objectRotationOffset;
        current.transform.localEulerAngles += new Vector3(0f, 0f, randomOrderStimuli.getUprightInverted());


        Vector3 refSize = startObjects.objects[0].GetComponent<Renderer>().bounds.size;
        float resizeX = refSize.x / current.GetComponent<Renderer>().bounds.size.x;
        float resizeY = refSize.y / current.GetComponent<Renderer>().bounds.size.y;
        float resizeZ = refSize.z / current.GetComponent<Renderer>().bounds.size.z;
        resizeX *= current.transform.localScale.x;
        resizeY *= current.transform.localScale.y;
        resizeZ *= current.transform.localScale.z;
        current.transform.localScale = new Vector3(resizeX, resizeY, resizeZ);

        //current.transform.localScale = Vector3.Scale(current.transform.localScale, destination.transform.localScale);


		// return the target to its original parent (we'll revert other values later)
		// this way it won't track with the "head" of the avatar
		current.transform.parent = parent;

        // but Turn on the current object
        current.SetActive(true);



		saveLayer = current.layer;
		setLayer(current.transform, viewLayer);
		hud.setMessage(current.name);
		hud.ForceShowMessage();

		// Here, we want to log the name of the stimulus and whether it is ----
		// upright or inverted (DJH) ------------------------------------------
		log.log("ViewingStimulusTimed\t" + current.name + "\t" + randomOrderStimuli.getUprightInverted(), 1);

	}

	public override void TASK_ADD(GameObject go, string txt) {
		if (txt == "add") {
			saveLayer = go.layer;
			setLayer(go.transform,viewLayer);
		} else if (txt == "remove") {
			setLayer(go.transform,saveLayer);
		}

	}

	public void returnCurrent() {
		current.transform.position = position;
		current.transform.localRotation = rotation;
		current.transform.localScale = scale;
		setLayer(current.transform,saveLayer);

        // turn off the object we're returning before turning on the next one
        current.SetActive(false);
    }

	public override void endTask() {
		//returnCurrent();
		//startObjects.current = 0;
		TASK_END();
	}

	public override void TASK_END() {

		base.endTask();

		trial_start = Experiment.Now();
		// increment the trial counter within LM_RandomOrderStimuli.cs --------
		randomOrderStimuli.incrementCurrent();
		// and set the current index in the startObjects list accordingly -----
		startObjects.current = randomOrderStimuli.getCurrentGameObjectIndex();
		// return the current game object -------------------------------------
		returnCurrent();
		// update to the next object ------------------------------------------
		current = startObjects.currentObject();
		// set first_frame to true for the next trial -------------------------
		first_frame = true;

		if (vrEnabled)
		{
			hud.hudPanel.transform.position = initialHUDposition;
		}
		else
		{
			// Change the anchor points to put the message back in center
			RectTransform hudposition = hud.hudPanel.GetComponent<RectTransform>() as RectTransform;
			hudposition.pivot = new Vector2(0.5f, 0.5f);
		}

        // turn on all targets
        foreach (GameObject item in startObjects.objects)
        {
            item.SetActive(true);
        }

		if (restrictMovement)
		{
			manager.player.GetComponent<CharacterController>().enabled = true;
			if (avatar.GetComponent<FirstPersonController>()) avatar.GetComponent<FirstPersonController>().enabled = true;
			manager.scaledPlayer.GetComponent<ThirdPersonCharacter>().immobilized = false;
		}
	}
    public void setLayer(Transform t, int l) {
		t.gameObject.layer = l;
		foreach (Transform child in t) {
			setLayer(child,l);
		}
	}
}
