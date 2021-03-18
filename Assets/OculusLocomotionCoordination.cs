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

    public bool debugLocomotion = true;

    public GameObject anch_ovrpc;
    public GameObject anch_camrig;
    public GameObject anch_trackspace;
    public GameObject anch_centeri;

    private int debug = 0;

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
        
        // ---------- DEBUGGING --------- //
        if(debugLocomotion){
            if(debug > 100){
                anch_ovrpc.transform.position = GameObject.Find("OVRPlayerController").transform.position;
                anch_camrig.transform.position = GameObject.Find("OVRCameraRig").transform.position;
                anch_trackspace.transform.position = GameObject.Find("TrackingSpace").transform.position;
                anch_centeri.transform.position = GameObject.Find("TrackingSpace/CenterEyeAnchor").transform.position;

                Debug.Log("OVRPlayerController");
                Debug.Log(GameObject.Find("OVRPlayerController").transform.rotation);
                Debug.Log("OVRCameraRig");
                Debug.Log(GameObject.Find("OVRCameraRig").transform.rotation);
                Debug.Log("TrackingSpace");
                Debug.Log(GameObject.Find("TrackingSpace").transform.rotation);
                Debug.Log("CenterEyeAnchor");
                Debug.Log(GameObject.Find("TrackingSpace/CenterEyeAnchor").transform.rotation);

                debug = 0;
            }
            debug += 1;
        }
    }
}
