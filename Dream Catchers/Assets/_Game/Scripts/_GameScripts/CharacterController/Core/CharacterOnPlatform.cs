using UnityEngine;
using System.Collections;

public class CharacterOnPlatform : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {        
        if (other.transform.tag == "Platform")
        {
            transform.parent = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Platform")
        {
            transform.parent = null;
        }
    }
}
