using UnityEngine;
using System.Collections;

public class HammerAttack : MonoBehaviour {

    public PlayerCombat combatScript;
    public int hammerDamage;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && combatScript.attacking && ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            other.BroadcastMessage("TakeDamage", hammerDamage);
        }
        if (other.tag == "PushBlock" && ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            other.BroadcastMessage("Push");
        }
    }
}
