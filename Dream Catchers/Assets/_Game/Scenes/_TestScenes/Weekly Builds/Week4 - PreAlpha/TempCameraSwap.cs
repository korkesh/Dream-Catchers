using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TempCameraSwap : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void enableSmartCam()
    {
        Game_Manager.instance.enableSmartCam = this.gameObject.GetComponent<Toggle>().isOn;

    }

}
