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

	public bool jitter_iti = true;
	public int jitter_lower_bound = 1500;
	public int jitter_upper_bound = 3000;
	public ObjectList startObjects;
	public bool blackout = true;
	public bool showName = false;
	private long trial_start;
    public bool restrictMovement = true;


    private Vector3 initialHUDposition;

    public override void startTask () {
		TASK_START();
			
		trial_start = Experiment.Now();
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

			// Put the message below the ground, so that they cannot see it for the ITI task
			var temp = destination.transform.position;
			temp.y -= 100f;
			hud.hudPanel.transform.position = temp;

		}
		else
		{
			// Put the message below the ground, so that they cannot see it for the ITI task
			RectTransform hudposition = hud.hudPanel.GetComponent<RectTransform>() as RectTransform;
			hudposition.pivot = new Vector2(0.5f, 100f);
		}
		        

        // turn off all objects
        foreach (GameObject item in startObjects.objects)
        {
            item.SetActive(false);
        }

		// if we want to jitter, then call that function to update the interval
        // parameter; otherwise, use the stock interval value from the
        // inspector (DJH)
		if (jitter_iti)
		{
			interval = jitterTrialEnd();
			// Debug.Log(interval);
		}
    }
	
	public override bool updateTask () {
		
		if (skip) {
			//log.log("INFO	skip task	" + name,1 );
			return true;
		}

		// Note, in the TASK_START() function above, I set the interval to
		// either be set by jitter or the interval, depending on user's preference.
		if ( Experiment.Now() - trial_start >= interval)
		{
			return true;
		} 
		return false;
	}
	
	public override void endTask() {
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

	private int jitterTrialEnd()
    {
		return Random.Range(jitter_lower_bound, jitter_upper_bound);
    }
}
