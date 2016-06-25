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

    void FixedUpdate()
    {
        //gameObject.GetComponent<Rigidbody>().velocity = pushTo / Time.deltaTime;

        //gameObject.GetComponent<Rigidbody>().MovePosition(pushTo * (Time.time - savedTime));
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
        else
        {

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
        pushDir = transform.position - Player.transform.position; //Player.GetComponent<PlayerMachine>().facing.normalized;
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

    }
}
