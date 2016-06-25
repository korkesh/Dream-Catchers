using UnityEngine;
using System.Collections;

public class CameraSwap : MonoBehaviour
{
    public enum Mode
    {
        Smart,
        Manual
    }

    private Mode mode;

    private RootCamera rootCam;
    private RealCamera viewCam;
    private TestCamera manualCam;
     
	// Use this for initialization
	void Start ()
    {
        mode = Mode.Smart;

        rootCam = GetComponent<RootCamera>();
        viewCam = GetComponentInChildren<RealCamera>();
        manualCam = GetComponent<TestCamera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if ( (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.X)) || (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.X)) )
        {
            if (mode == Mode.Smart)
            {
                mode = Mode.Manual;

                rootCam.GetComponent<Camera>().targetDisplay = 0;
                viewCam.GetComponent<Camera>().targetDisplay = 1;
                manualCam.enabled = true;
            }
            else
            {
                mode = Mode.Smart;

                rootCam.GetComponent<Camera>().targetDisplay = 1;
                viewCam.GetComponent<Camera>().targetDisplay = 0;
                manualCam.enabled = false;
            }
        }
	}
}
