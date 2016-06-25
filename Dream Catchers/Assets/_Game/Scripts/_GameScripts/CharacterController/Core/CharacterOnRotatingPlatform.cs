///===============================================================================
/// Author: Matt
/// Purpose: Prevents the player character from falling off of rotating platforms
///          This code will likely end up being consolidated into the player
///          machine script
///===============================================================================

using UnityEngine;
using System.Collections;

public class CharacterOnRotatingPlatform : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {        
        if (other.transform.tag == "Platform")
        {
            transform.parent = other.transform; // Set character as child of platform object
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Platform")
        {
            transform.parent = null; // Remove platform as parent object
        }
    }
}
