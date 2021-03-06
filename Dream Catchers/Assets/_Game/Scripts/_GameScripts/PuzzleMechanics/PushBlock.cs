﻿///=====================================================================================
/// Author: Matt
/// Purpose: Pushes a block along the ground based on a calculated velocity
///======================================================================================

using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class PushBlock : MonoBehaviour {

    public float pushDistance = 2.0f;
    public float pushTime = 0.5f;

    public bool playerPush;

    public GameObject Player;

    Vector3 pushDir;
    Vector3 pushTo;
    float savedTime;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        pushDir = Vector3.zero;
        pushTo = Vector3.zero;
    }


    void OnCollisionEnter(Collision collision)
    {
        //If the box is moving and collides with the wall, stop the rolling sound and stop sound
        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude < 0.1f && GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Stop();
        }

        if(gameObject.GetComponent<Rigidbody>().velocity.magnitude > 1.0f)
        {
            return;
        }

        if (gameObject.GetComponent<ManipulationGravity>() != null && gameObject.GetComponent<ManipulationGravity>().currentFloat != ManipulationGravity.FLOAT_STATE.NEUTRAL)
        {
            return;
        }

        // Only allow push from player and when not animating
        if (collision.gameObject.tag == "Player")
        {
            if(playerPush)
            {
                Push();
            }
            else
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }


    /// <summary>
    /// Pushes a box in the direction the character is facing by the pushpower amount over pushtime seconds.
    /// The box will not be pushed if the character comes at it from an angle.
    /// </summary>
    public void Push()
    {
        // Disallow a second push when in motion
        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > 1.0f)
        {
            return;
        }

        // Disallow push if floating box
        if (gameObject.GetComponent<ManipulationGravity>() != null && gameObject.GetComponent<ManipulationGravity>().currentFloat != ManipulationGravity.FLOAT_STATE.NEUTRAL)
        {
            return;
        }

        gameObject.GetComponent<Rigidbody>().useGravity = true;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        // The direction the character is facing when colliding with the box
        pushDir = transform.position - Player.transform.position; 
        pushDir.y = 0;

        // If the character is not head on do not push
        if (Mathf.Abs(pushDir.x) > Mathf.Abs(pushDir.z))
        {
            pushDir.z = 0;
        }
        else
        {
            pushDir.x = 0;
        }

        pushDir.Normalize();

        // The position to push the box to
        pushTo = new Vector3(pushDir.x * pushDistance, 0, pushDir.z * pushDistance);

        gameObject.GetComponent<Rigidbody>().velocity = pushTo / pushTime;

        gameObject.SendMessage("Play"); // Sound effect

    }
}
