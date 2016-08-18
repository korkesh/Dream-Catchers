using UnityEngine;
using System.Collections;

public class SkyBoxStay : MonoBehaviour {

    //main menu .. keep the sky box from changing

    bool foundIt;
    GameObject mainCamera;
    public Material skyboxNightmare;

	// Use this for initialization
	void Start () {
	
        if(foundIt == false)
        {
            mainCamera = Camera.main.gameObject;
            if (mainCamera != null)
            {
                mainCamera.GetComponent<Skybox>().material = skyboxNightmare;
                foundIt = true;
            }
        }
	}
	
	void Awake () {

        foundIt = false;
        mainCamera = Camera.main.gameObject;
        if(mainCamera != null)
        {
            mainCamera.GetComponent<Skybox>().material = skyboxNightmare;
            foundIt = true;
        }
	
	}
}
