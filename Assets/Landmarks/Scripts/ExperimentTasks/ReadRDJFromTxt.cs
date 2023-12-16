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
MODIFIED BY AK BONIN (DECEMBER 2023)
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public class ReadRDJFromTxt : ExperimentTask {

	public EndListMode EndListBehavior; 
	public int size = 9999;
    public int subjNum;

	private List<List<string>> objList;
    private List<string> trial;

	private int current = 0;
	public string currentTrial = "";


	public override void startTask () {
		TASK_START();

        string filename = "Assets/Landmarks/TextFiles/ParticipantFiles/s" + subjNum.ToString() + "_paths.txt";
		string[] objs = System.IO.File.ReadAllLines(filename);

		int eachLine;
        trial = new List<string>();
        int trialNum = 1;
		for ( eachLine = 0; eachLine < objs.Length; eachLine++ )
		{
            if ( string.IsNullOrWhiteSpace(objs[eachLine]) ) {
                // save previous trial
                objList.Add(trial);
                trialNum++;
                Debug.Log("Trial" + trialNum.ToString() + "\n");
                trial = new List<string>();
            } else {
                Debug.Log(objs[eachLine]  + "\n");
			    trial.Add(objs[eachLine]);
            }
			
		}

		
		objList = objList.GetRange(0, eachLine);
		foreach( List<string> o in objList ) {
			//Debug.Log(txt);
			log.log("TASK_ADD	" + name  + "\t" + this.GetType().Name + "\t" + name  + "\t" + o,1 );

		}

		currentTrial = currentString();


	}	

	public override void TASK_ADD(GameObject go, string txt) {
		Debug.Log("ADD  " + txt);
        List<string> txtList = new List<string>();
        txtList.Add(txt);
		objList.Add(txtList);
	}

	public override void TASK_START()
	//public  void Awake()
	{
		base.startTask();

		if (!manager) Start();
		objList = new List<List<string>>();


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
		if (current >= objList.Count) {
			return null;
		}
        
        int eachLine;
        string trialString = "";
        for (eachLine = 0; eachLine < objList[current].Count; eachLine++ ) {
            trialString += objList[current][eachLine] + ",";
        }
        
		return trialString;
	}

	public override void incrementCurrent() {
		current++;
		if (current >= objList.Count && EndListBehavior == EndListMode.Loop) {
			current = 0;
		}
		currentTrial = currentString();
	}
}