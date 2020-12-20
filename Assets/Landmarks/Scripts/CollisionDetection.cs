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

public class CollisionDetection : MonoBehaviour {

	private Experiment manager;
	private dbLog log;


	void OnControllerColliderHit(ControllerColliderHit hit)  {
        //Debug.Log("OnControllerColliderHit-CollisionDetection.cs");
		if(hit.gameObject.tag == "Target") {
                    Debug.Log("OnControllerColliderHit-CollisionDetection.cs-isTarget");
			manager.OnControllerColliderHit(hit.gameObject);
		}   
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter-CollisionDetection.cs");
        if(collision.gameObject.tag == "Target")
        {
            manager.OnControllerColliderHit(collision.gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("OnTriggerEnter-CollisionDetection.cs");
        manager.OnControllerColliderHit(other.gameObject);
    }
    
    
    void Start ()
	{		
		GameObject experiment = GameObject.FindWithTag ("Experiment");
	    manager = experiment.GetComponent("Experiment") as Experiment;
	    log = manager.dblog;
	}
	
	
	


}

