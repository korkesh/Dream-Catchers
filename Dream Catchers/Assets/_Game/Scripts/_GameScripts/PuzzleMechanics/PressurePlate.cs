using UnityEngine;
using System.Collections;

public class PressurePlate : MonoBehaviour {

    public string WeightedObjectTag;

    public GameObject ObjectToTrigger;
    public string TriggerFunctionCall;

    // TODO: Add animator? How do we want to handle lowering of plate?
    public GameObject PressurePlateObject;
    public float animationSpeed;

    Vector3 originalPos;
    Vector3 destinationPos;
    float timer;

    void Start()
    {
        originalPos = PressurePlateObject.transform.position;
        destinationPos = PressurePlateObject.transform.position;
        timer = Time.deltaTime;
    }

    void Update()
    {
        // From position1 to position2 with increasing t 
        PressurePlateObject.transform.position = Vector3.Lerp(originalPos, destinationPos, animationSpeed * timer);

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

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == WeightedObjectTag)
        {
            Debug.Log("Player on plate");

            timer = Time.deltaTime;
            originalPos = PressurePlateObject.transform.position;
            destinationPos.y = -destinationPos.y;

            ObjectToTrigger.SendMessage(TriggerFunctionCall);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == WeightedObjectTag)
        {
            Debug.Log("Player no longer on plate");

            timer = Time.deltaTime;
            originalPos = PressurePlateObject.transform.position;
            destinationPos.y = -destinationPos.y;

            ObjectToTrigger.SendMessage(TriggerFunctionCall);
        }
    }

}
