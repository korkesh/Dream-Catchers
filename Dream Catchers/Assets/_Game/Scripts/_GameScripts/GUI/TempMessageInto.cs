//================================
// Alex
//  temp script for little into scene in new game
//================================
using UnityEngine;
using System.Collections;

public class TempMessageInto : MonoBehaviour {

    public string nextscene;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetButtonDown("Jump") == true)
        {
            Game_Manager.instance.ChangeScene(nextscene);
        }

	}
}
