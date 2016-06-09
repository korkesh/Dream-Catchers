using UnityEngine;
using System.Collections;

public class ScneScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {

        Level_Manager.instance.ContinueLevel();
        Character_Manager.instance.GoTocheckPoint();
	
	}
	
}
