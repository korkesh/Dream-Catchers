//================================
// Alex
//  temp script for a scene load
//================================

using UnityEngine;
using System.Collections;

public class SceneScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {

        Level_Manager.Instance.ContinueLevel();
        Character_Manager.instance.GoTocheckPoint();
	
	}
	
}
