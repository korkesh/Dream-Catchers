///=====================================================================================
/// Author: Matt
/// Purpose: Animates the lowering of a door
///======================================================================================

using UnityEngine;
using System.Collections;

public class LowerDoor : MonoBehaviour {

    public Vector3 StartPos;
    public Vector3 EndPos;
    public float animationSpeed;

    Vector3 originalPos;
    Vector3 destinationPos;
    float timer;
    bool open = false;
    public bool isTrigger = false;
    public bool isCinematic = false;
    bool startCinematic = false;

    // Use this for initialization
    void Start () {
        timer = Time.deltaTime;
        originalPos = transform.position;

        if(!isCinematic)
        {
            destinationPos = StartPos;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if ((isCinematic && startCinematic) || !isCinematic)
        {
            // Smooth learp animation from original pos to destination pos
            transform.position = Vector3.Lerp(originalPos, destinationPos, animationSpeed * timer);

            // Increase or decrease the constant lerp timer 
            if (destinationPos == originalPos)
            {
                timer = Mathf.Clamp(timer - Time.deltaTime, 0.0f, 1.0f / animationSpeed);
            }
            else
            {
                timer = Mathf.Clamp(timer + Time.deltaTime, 0.0f, 1.0f / animationSpeed);
            }
        }
    }

    // Toggle close and open of door
    public void ActivateDoor()
    {
        open = !open;

        if(open)
        {
            timer = Time.deltaTime;
            originalPos = transform.position;
            destinationPos = EndPos;
        }
        else
        {
            timer = Time.deltaTime;
            originalPos = transform.position;
            destinationPos = StartPos;
        }

    }

    public void CloseDoor()
    {
        timer = Time.deltaTime;
        transform.position = StartPos;
        originalPos = transform.position;
        destinationPos = EndPos;

        if (isCinematic)
        {
            startCinematic = true;
        }
    }

    public void OpenDoor()
    {
        timer = Time.deltaTime;
        transform.position = EndPos;
        originalPos = transform.position;
        destinationPos = StartPos;

        if(isCinematic)
        {
            startCinematic = true;
        }
    }
}
