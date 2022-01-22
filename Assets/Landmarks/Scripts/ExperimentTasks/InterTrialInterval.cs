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
*/

using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Characters.FirstPerson;


public class InterTrialInterval : ExperimentTask {

    [Header("Task-specific Properties")]

	[HideInInspector] public GameObject destination;

	public ObjectList startObjects;
	private static Vector3 position;
	private static Quaternion rotation;
	private static Transform parent;
	private static Vector3 scale;
	private int saveLayer;
	private int viewLayer = 11;
	public bool blackout = true;
	public bool showName = false;
	private long trial_start;
    private float trial_start_float;
	public Vector3 objectRotationOffset;
	public Vector3 objectPositionOffset;
    public bool restrictMovement = true;


    private Vector3 initialHUDposition;

    public override void startTask () {
		TASK_START();
			
		trial_start = Experiment.Now();
		trial_start_float = trial_start/1f;
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
			//if (showName)
			//{
			//	temp.y += 2.5f;
			//}
			//else
			//{
			//	temp.y -= 100f;
			//}
			temp.y -= 100f;
			hud.hudPanel.transform.position = temp;

		}
		else
		{
			// Change the anchor points to put the message at the bottom
			RectTransform hudposition = hud.hudPanel.GetComponent<RectTransform>() as RectTransform;
			// If showName, then put in place, otherwise move under the floor (DJH)
			//if (showName)
			//{
			//	hudposition.pivot = new Vector2(0.5f, 0.1f);
			//}
			//else
			//{
			//	hudposition.pivot = new Vector2(0.5f, 100f);
			//}
			hudposition.pivot = new Vector2(0.5f, 100f);
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
		
	
		if ( Experiment.Now() - trial_start >= interval)  {
			return true;
		} 
		return false;
	}
	
	public override void TASK_ADD(GameObject go, string txt) {
		if (txt == "add") {
			saveLayer = go.layer;
			setLayer(go.transform,viewLayer);
		} else if (txt == "remove") {
			setLayer(go.transform,saveLayer);
		}

	}
	
	public override void endTask() {
		//returnCurrent();
		//startObjects.current = 0;
		TASK_END();
	}
	
	public override void TASK_END() {

		base.endTask();

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
