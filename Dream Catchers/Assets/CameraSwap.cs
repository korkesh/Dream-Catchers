///=====================================================================================
/// Author: Connor
/// Purpose: Swamps between smart camera and temporary camera
///======================================================================================
///
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

    //private RootCamera rootCam;
    //private RealCamera viewCam;
    private NewCamera newCam;
    private TempCamera manualCam;
     
	// Use this for initialization
	void Start ()
    {
        mode = Mode.Smart;

        //rootCam = GetComponent<RootCamera>();
        //viewCam = GetComponentInChildren<RealCamera>();

        newCam = GetComponent<NewCamera>();
        manualCam = GetComponent<TempCamera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if ( (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.X)) || (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.X)) )
        {
            if (mode == Mode.Smart)
            {
                mode = Mode.Manual;

                newCam.enabled = false;
                manualCam.enabled = true;
            }
            else
            {
                mode = Mode.Smart;

                newCam.enabled = true;
                manualCam.enabled = false;
            }
        }
	}
}
