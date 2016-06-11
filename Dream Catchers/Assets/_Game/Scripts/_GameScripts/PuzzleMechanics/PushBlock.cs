﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class PushBlock : MonoBehaviour {

    public float pushPower = 2.0f;
    public float pushTime = 0.5f;

    public bool playerPush;

    public GameObject Player;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnCollisionEnter(Collision collision)
    {
        if(gameObject.GetComponent<Rigidbody>().velocity.magnitude > 1.0f)
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
        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > 1.0f)
        {
            return;
        }

        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        // The direction the character is facing when colliding with the box
        Vector3 pushDir = Player.GetComponent<PlayerMachine>().facing.normalized;

        // If the character is not head on do not push
        if (Mathf.Round(pushDir.x) != 0 && Mathf.Round(pushDir.z) != 0)
        {
            return;
        }

        // The position to push the box to
        Vector3 pushTo = transform.position + new Vector3(Mathf.Round(pushDir.x) * pushPower, 0, Mathf.Round(pushDir.z) * pushPower);

        // The push animation
        gameObject.GetComponent<Rigidbody>().velocity = pushDir * pushPower;
        
    }
}
