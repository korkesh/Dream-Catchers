using UnityEngine;
using System.Collections;

public class EyeTracking : MonoBehaviour {

    public GameObject playerTracker;
	
	// Update is called once per frame
	void Update () {
	
        if(playerTracker)
        {
            transform.LookAt(playerTracker.transform, playerTracker.transform.up);
        }

	}
}
