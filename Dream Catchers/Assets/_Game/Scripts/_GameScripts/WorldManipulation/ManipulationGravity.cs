///=====================================================================================
/// Author: Matt
/// Purpose: Handles changes in gravity on an object upon world swap; seen in 
/// floating balloons
///======================================================================================

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ManipulationGravity : ManipulationScript {

    public enum FLOAT_STATE
    {
        UP = 0,
        DOWN = 1,
        NEUTRAL = 2,
        BLOCK = 3
    }

    public FLOAT_STATE currentFloat = FLOAT_STATE.DOWN;

    public float maxDist; // Max height to float
    public float minDist; // Min height to float
    public float timeToFloat; // Time it takes to float between min/max
    public float minY; // Min Y posistion

    // Toggle which state to float in
    public bool floatInDream;
    public bool floatInNightmare;

    // Is a pushblock as well
    public bool isPushBlock;

    // Use this for initialization
    void Start()
    {
        // Set the default world state
        currentManipType = MANIPULATION_TYPE.OTHER;
    }

    void FixedUpdate()
    {
        // Handle cases where the object has finished moving from either up or down state
        if (gameObject.GetComponent<Rigidbody>().velocity == Vector3.zero)
        {
            // If push block as well, set proper rigidbody
            if (gameObject.tag == "PushBlock")
            {
                gameObject.GetComponent<Rigidbody>().useGravity = false;
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }

            if (currentFloat == FLOAT_STATE.DOWN)
            {
                FloatDown();
            }
            else if (currentFloat == FLOAT_STATE.UP)
            {
                FloatUp();
            }
        }
        else
        {
            //Debug.Log(gameObject.GetComponent<Rigidbody>().velocity);
        }
    }

    // Float box upwards
    void FloatUp()
    {
        Vector3 dir = new Vector3(0, -1, 0);
        RaycastHit hit;

        int layerMask = 1 << 9; // Only float Above the level mask

        Debug.DrawRay(transform.position, dir * maxDist);

        Physics.Raycast(transform.position, dir, out hit, layerMask); // The Raycast to calculate distance from floor

        if (maxDist - hit.distance > 0.01 && hit.distance >= minDist && !DOTween.IsTweening(transform)) 
        {
            // Move by offset distance
            Vector3 toMove = new Vector3(transform.position.x, transform.position.y + (maxDist - hit.distance), transform.position.z);
            transform.DOMove(toMove, timeToFloat); // Move box using tween
        }
    }

    // Float box downwards
    void FloatDown()
    {
        Vector3 dir = new Vector3(0, -1, 0);
        RaycastHit hit;

        int layerMask = 1 << 9; // Only float Above the level mask

        Debug.DrawRay(transform.position, dir * maxDist);

        Physics.Raycast(transform.position, dir, out hit, layerMask); // The Raycast to calculate distance from floor

        if (hit.distance != minDist && !DOTween.IsTweening(transform))
        {
            // Move by offset distance
            Vector3 toMove = new Vector3(transform.position.x, transform.position.y + (minDist - hit.distance), transform.position.z);
            if(toMove.y < minY)
            {
                toMove.y = minY;
            }
            transform.DOMove(toMove, timeToFloat); // Move box using tween
        }
    }

    // Set the box state based on world state
    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

        DOTween.Kill(transform);

        if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM && floatInDream)
        {
            currentFloat = FLOAT_STATE.UP;
        }
        else if (currentObjectState == ManipulationManager.WORLD_STATE.NIGHTMARE && floatInNightmare)
        {
            currentFloat = FLOAT_STATE.UP;
        }
        else
        {
            currentFloat = FLOAT_STATE.DOWN;
        }

    }

    // Box has hit the floor or another object
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") // Prevent character from being crushed
        {
            if (currentFloat == FLOAT_STATE.DOWN)
            {
                currentFloat = FLOAT_STATE.BLOCK;
                DOTween.Pause(transform);
            }
            return;
        }
        currentFloat = FLOAT_STATE.NEUTRAL;
        DOTween.Kill(transform);
    }

    // Box has hit left floor or another object
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (currentFloat == FLOAT_STATE.BLOCK)
            {
                currentFloat = FLOAT_STATE.DOWN;
                DOTween.Play(transform);
            }
            return;
        }

    }

}
