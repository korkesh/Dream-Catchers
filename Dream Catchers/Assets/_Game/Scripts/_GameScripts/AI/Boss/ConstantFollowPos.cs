using UnityEngine;
using System.Collections;

public class ConstantFollowPos : MonoBehaviour {

    public Vector3 Follow;
    public Transform Object;
	
	// Update is called once per frame
	void Update () {

        //follows an object and stays at a certain spot relative to it
        this.transform.position = Object.position + Follow;
	}
}
