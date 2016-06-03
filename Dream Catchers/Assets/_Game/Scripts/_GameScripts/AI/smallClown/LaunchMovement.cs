using UnityEngine;
using System.Collections;

public class LaunchMovement : MonoBehaviour {

     public GameObject clown;
     public float distanceInFront;

	// Use this for initialization
	void Awake () {

        Vector3 parentPOs = clown.transform.position;
        parentPOs.y = this.transform.position.y;
        transform.position = parentPOs + clown.transform.forward*distanceInFront;
        transform.forward = clown.transform.forward;


	}
	
	// Update is called once per frame
	void Update () {
        transform.forward = clown.transform.forward;
	}
}
