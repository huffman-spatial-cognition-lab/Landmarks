using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetObjectScript : MonoBehaviour
{

    private RelocationTask relocationTask;

    // Start is called before the first frame update
    void Start()
    {
        relocationTask = GameObject.Find("PTOTrials/Relocate").GetComponent<RelocationTask>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if(other.gameObject.name.Contains("GrabVolume"))
        {
            Debug.Log("targetObjectScript-OnTriggerEnter-otherColliderNameContainsHand");
            // if so, call the RelocationTask function for this.
            relocationTask.completeCurrentObject();
        }
    }
}
