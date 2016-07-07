using UnityEngine;
using System.Collections;


public class NewCamera : MonoBehaviour
{
    //===========================================
    // Fields
    //===========================================

    // Modes
    public enum CameraMode
    {
        High,
        Low
    }

    public CameraMode Mode { get; private set;}

    // Player:
    public GameObject Player;
    private Transform Target;

    private Vector3 DisplacementDir; // direction from cam to player

    //==========================================
    // Constraints:
    //==========================================
    public float lastGround; // y value of ground either previously stood on or currently slightly above

    public float hFollowDistance;
    public float lFollowDistance;

    // Height offset from LastGround position
    public float hHeight;
    public float lHeight;






    //********************************************************************
    //*
    //*   Abstract
    //*
    /*
     *   Follow distance is constant
     *   
     */

	void Start ()
    {
        Mode = CameraMode.High;
	}
	


	void Update ()
    {
	
	}
}
