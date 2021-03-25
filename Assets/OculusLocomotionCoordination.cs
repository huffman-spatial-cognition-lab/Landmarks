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
    public GameObject anch_charcont;
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
        Vector3 CenterPosition = GameObject.Find("TrackingSpace/CenterEyeAnchor").transform.position;
        CharacterController cc = GameObject.Find("OVRPlayerController").GetComponent(typeof(CharacterController)) as CharacterController;
        cc.center = new Vector3(CenterPosition.x, 0.2, CenterPosition.z);

        // ---------- DEBUGGING --------- //
        if(debugLocomotion){
            anch_ovrpc.transform.position = GameObject.Find("OVRPlayerController").transform.position;
            anch_charcont.transform.position = new Vector3(CenterPosition.x, 0.2, CenterPosition.z);
            anch_trackspace.transform.position = GameObject.Find("TrackingSpace").transform.position;
            anch_centeri.transform.position = GameObject.Find("TrackingSpace/CenterEyeAnchor").transform.position +  GameObject.Find("TrackingSpace/CenterEyeAnchor").transform.forward * 0.5f;
            if(debug > 50){
                

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
