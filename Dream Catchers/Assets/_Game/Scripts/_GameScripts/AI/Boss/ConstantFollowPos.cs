using UnityEngine;
using System.Collections;

public class ConstantFollowPos : MonoBehaviour {

    public Vector3 Follow;
    public Transform Object;
	
	// Update is called once per frame
	void Update () {

        this.transform.position = Object.position + Follow;
	}
}
