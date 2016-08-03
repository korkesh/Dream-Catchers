///=====================================================================================
/// Author: Matt
/// Purpose: Causes hammer to deal damage or push blocks depending on world state
///======================================================================================

using UnityEngine;
using System.Collections;

public class HammerAttack : MonoBehaviour {

    public PlayerCombat combatScript;
    public int hammerDamage;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && combatScript.damage && ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            other.BroadcastMessage("TakeDamage", hammerDamage);
        }
        if (other.tag == "PushBlock" && ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            other.BroadcastMessage("Push");
        }
    }
}
