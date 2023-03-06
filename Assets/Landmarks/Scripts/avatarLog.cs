using UnityEngine;
using System.Collections;

public class avatarLog : MonoBehaviour {

	[HideInInspector] public bool navLog = false;
	private Transform avatar;
	private Transform cameraCon;
	private Transform cameraRig;

	private GameObject experiment;
	private dbLog log;
	private Experiment manager;
	
	public GameObject player;
	public GameObject camerarig;

	void Start () {

		cameraCon =player.transform as Transform;
		cameraRig =camerarig.transform as Transform;

		experiment = GameObject.FindWithTag ("Experiment");
		manager = experiment.GetComponent("Experiment") as Experiment;
		log = manager.dblog;
		avatar = transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        // Log the name of the tracked object, it's body position, body rotation, and camera (head) rotation
		if (navLog){
			/* PROTOTYPE: how to reconstruct
			

			float lookDist = cameraCon.eulerAngles.y / cameraCon.transform.forward.y;
			if (lookDist <= 0) {
				log.log("HEADSET LOOKING UP")
			} else {
				log.log("HEADSET LOOKING AT Y=0 AT\t" + (cameraCon.position.x + cameraCon.transform.forward.x * lookDist) +  "\t" + (cameraCon.position.z + cameraCon.transform.forward.z * lookDist));
			}
			
			*/
            //print("AVATAR_POS	" + "\t" +  avatar.position.ToString("f3") + "\t" + "AVATAR_Body " + "\t" +  cameraCon.localEulerAngles.ToString("f3") +"\t"+ "AVATAR_Head " + cameraRig.localEulerAngles.ToString("f3"));
            log.log("Avatar: \t" + avatar.name + "\t" +
                    "Position (xyz): \t" + cameraCon.position.x + "\t" + cameraCon.position.y + "\t" + cameraCon.position.z + "\t" +
                    "Rotation (xyz): \t" + cameraCon.eulerAngles.x + "\t" + cameraCon.eulerAngles.y + "\t" + cameraCon.eulerAngles.z + "\t" +
                    "Forward   (xyz): \t" + cameraCon.transform.forward.x + "\t" + cameraCon.transform.forward.y + "\t" + cameraCon.transform.forward.z + "\t"
                    , 1);
        }
	}
}
