///=====================================================================================
/// Author: Matt
/// Purpose: Detects if the player has fallen off of the level and kills the character
///======================================================================================

using UnityEngine;
using System.Collections;

public class KillFloor : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Character_Manager.instance.takeDamage(Character_Manager.instance.maxHealth);
        }
    }
}
