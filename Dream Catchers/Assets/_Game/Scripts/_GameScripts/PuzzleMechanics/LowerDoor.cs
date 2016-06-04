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

    // Use this for initialization
    void Start () {
        timer = Time.deltaTime;
        originalPos = transform.position;
        destinationPos = openPos;
    }
	
	// Update is called once per frame
	void Update () {

        // From position1 to position2 with increasing t 
        transform.position = Vector3.Lerp(originalPos, destinationPos, animationSpeed * timer);

        // Increase or decrease the constant lerp timer 
        if (destinationPos == originalPos)
        {
            // Go to position1 t = 0.0f
            timer = Mathf.Clamp(timer - Time.deltaTime, 0.0f, 1.0f / animationSpeed);
        }
        else
        {
            // Go to position2 t = 1.0f
            timer = Mathf.Clamp(timer + Time.deltaTime, 0.0f, 1.0f / animationSpeed);
        }
    }

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
