///=====================================================================================
/// Author: Matt
/// Purpose: Detects if the palyer is crushed via a break in a placed joint. 
///          Has too many false positives, currently looking into alternative method.
///======================================================================================

using UnityEngine;
using System.Collections;

public class CharacterCrush : MonoBehaviour {

    void OnJointBreak(float breakForce)
    {
        Debug.Log("A joint has just been broken!, force: " + breakForce);

        Character_Manager.instance.takeDamage(Character_Manager.instance.maxHealth);
        Destroy(gameObject);
    }
}
