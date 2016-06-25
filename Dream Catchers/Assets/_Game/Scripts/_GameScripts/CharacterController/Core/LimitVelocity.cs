///==========================================================================
/// Author: Matt
/// Purpose: Places an upperbound on the velocity of the player character
///          in all directions. Used to prevent the character from clipping
///          through objects at high speeds.
///==========================================================================

using UnityEngine;
using System.Collections;

public class LimitVelocity : MonoBehaviour {

    public Vector3 maxVelocity; // Upper-Bound Velocities

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerMachine pm = gameObject.GetComponent<PlayerMachine>(); // Used to access character's current velocity

        // Clamp X velocity
        if (Mathf.Abs(pm.moveDirection.x) > maxVelocity.x)
        {
            Debug.Log("Slow DOWN! (X) " + pm.moveDirection.x);
            if(pm.moveDirection.x > 0)
            {
                pm.moveDirection.x = maxVelocity.x;
            }
            else
            {
                pm.moveDirection.x = -maxVelocity.x;
            }
        }

        // Clamp Y Velocity
        if (Mathf.Abs(pm.moveDirection.y) > maxVelocity.y)
        {
            Debug.Log("Slow DOWN! (Y) " + pm.moveDirection.y);
            if (pm.moveDirection.y > 0)
            {
                pm.moveDirection.y = maxVelocity.y;
            }
            else
            {
                pm.moveDirection.y = -maxVelocity.y;
            }
        }

        // Clamp Z Velocity
        if (Mathf.Abs(pm.moveDirection.z) > maxVelocity.z)
        {
            Debug.Log("Slow DOWN! (Z) " + pm.moveDirection.z);
            if (pm.moveDirection.z > 0)
            {
                pm.moveDirection.z = maxVelocity.z;
            }
            else
            {
                pm.moveDirection.z = -maxVelocity.z;
            }
        }      
    }
}
