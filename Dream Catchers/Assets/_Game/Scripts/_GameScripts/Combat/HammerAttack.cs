using UnityEngine;
using System.Collections;

public class HammerAttack : MonoBehaviour {

    public PlayerCombat combatScript;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && combatScript.attacking)
        {
            Destroy(other.gameObject);
        }
    }
}
