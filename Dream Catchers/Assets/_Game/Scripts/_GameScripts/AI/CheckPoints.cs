//================================
// Alex
//  checkpoint for players. Save postion to playerPrefs
//================================

using UnityEngine;
using System.Collections;

public class CheckPoints : MonoBehaviour {

    public Vector3 position;
    public bool useCurrentPos;

	// Use this for initialization
	void Start () {
	
        if(useCurrentPos == true)
        {
            position = this.transform.position;
        }

	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Level_Manager.instance.newCheckPoint(position);
            //might remove
            Destroy(this.gameObject,0.5f);
        }
        
    }
}
