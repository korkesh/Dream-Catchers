//================================
// Alex
//  temp script for pause.
//================================
using UnityEngine;
using System.Collections;

public class TimePause : MonoBehaviour {

   

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void pause()
    {
       
    }

    public void unPause()
    {
        Time.timeScale = UI_Manager.instance.timePlaceHolder;
    }
}
