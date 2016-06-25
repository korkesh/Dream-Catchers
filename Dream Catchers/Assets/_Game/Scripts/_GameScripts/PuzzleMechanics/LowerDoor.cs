///=====================================================================================
/// Author: Matt
/// Purpose: Animates the lowering of a door
///======================================================================================

using UnityEngine;
using System.Collections;

public class LowerDoor : MonoBehaviour {

    public Vector3 openPos;
    public Vector3 closePos;
    public float animationSpeed;

    Vector3 originalPos;
    Vector3 destinationPos;
    float timer;
    bool open = false;
    public bool isTrigger = false;

    // Use this for initialization
    void Start () {
        timer = Time.deltaTime;
        originalPos = transform.position;
        destinationPos = openPos;
    }
	
	// Update is called once per frame
	void Update () {

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

    // Toggle close and open of door
    public void ActivateDoor()
    {
        open = !open;

        if(open)
        {
            timer = Time.deltaTime;
            originalPos = transform.position;
            destinationPos = closePos;
        }
        else
        {
            timer = Time.deltaTime;
            originalPos = transform.position;
            destinationPos = openPos;
        }

    }

}
