using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 

COORDINATES LOCOMOTION WITH THE OCULUS QUEST HEADSET.

To enable collision, we move the CharacterController (which facilitates collision with the world) around with the center camera,
which is always in sync with where the headset actually is.

*/

public class OculusLocomotionCoordination : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 CenterPosition = GameObject.Find("TrackingSpace/CenterEyeAnchor").transform.localPosition;
        CharacterController cc = GameObject.Find("OVRPlayerController").GetComponent(typeof(CharacterController)) as CharacterController;
        cc.center = CenterPosition;

    }
}
