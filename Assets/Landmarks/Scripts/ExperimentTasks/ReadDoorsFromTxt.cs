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

/*
MODIFICATIONS:

Changed this code to only include stores that
are currently active (and to ignore inactive
	stores). This allows me to activate/inactivate
stores in other scripts and to have the questions
modified accordingly.

EDITED BY DJ HUFFMAN (MARCH 2017)
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;




public class ReadDoorsFromTxt : ExperimentTask {

	//public string question;
	//public GameObject parentObject;
	public EndListMode EndListBehavior; 
	public int size = 9999;
    public InputParticipantID participantInfo;
    private int subjNum;

	private List<List<string>> doorList;
    private List<string> trial;

	private int current = 0;
	public string currentTrial = "";


	public override void startTask () {
		TASK_START();

		subjNum = participantInfo.participantID;
        string filename = "Assets/Landmarks/TextFiles/ParticipantFiles/s" + subjNum.ToString() + "_doors.txt";
		log.log("READING: " + filename, 1);
		string[] doors = System.IO.File.ReadAllLines(filename);

		int eachLine;
        trial = new List<string>();
        int trialNum = 1;
		for ( eachLine = 0; eachLine < doors.Length; eachLine++ )
		{
            if ( string.IsNullOrWhiteSpace(doors[eachLine]) ) {
                // save previous trial
                doorList.Add(trial);
                trialNum++;
                Debug.Log("Trial" + trialNum.ToString() + "\n");
                trial = new List<string>();
            } else {
                Debug.Log(doors[eachLine]  + "\n");
			    trial.Add(doors[eachLine]);
            }
			
		}

		
		doorList = doorList.GetRange(0, eachLine);
		foreach( List<string> d in doorList ) {
			//Debug.Log(txt);
			log.log("TASK_ADD	" + name  + "\t" + this.GetType().Name + "\t" + name  + "\t" + d,1 );

		}

		currentTrial = currentString();


	}	

	public override void TASK_ADD(GameObject go, string txt) {
		Debug.Log("ADD  " + txt);
        List<string> txtList = new List<string>();
        txtList.Add(txt);
		doorList.Add(txtList);
	}

	public override void TASK_START()
	//public  void Awake()
	{
		base.startTask();

		if (!manager) Start();
		doorList = new List<List<string>>();


	}

	public override bool updateTask () {
		return true;
	}
	public override void endTask() {
		TASK_END();
	}

	public override void TASK_END() {
		base.endTask();
	}

	public override string currentString() {
		if (current >= doorList.Count) {
			return null;
		}
        
        int eachLine;
        string trialString = "";
        for (eachLine = 0; eachLine < doorList[current].Count; eachLine++ ) {
            trialString += doorList[current][eachLine] + ",";
        }
        
		return trialString;
	}

	public override void incrementCurrent() {
		current++;
		if (current >= doorList.Count && EndListBehavior == EndListMode.Loop) {
			current = 0;
		}
		currentTrial = currentString();
	}
}