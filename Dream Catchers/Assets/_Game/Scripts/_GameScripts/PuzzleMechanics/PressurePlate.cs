using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour {

    public List<string> WeightedObjectTag;
    List<string> objectsOnSwitch;

    public GameObject ObjectToTrigger;
    public string TriggerFunctionCall;

    // TODO: Add animator? How do we want to handle lowering of plate?
    public GameObject PressurePlateObject;
    public float animationSpeed;

    public bool allowDeactivate;
    public bool activated = false;

    Vector3 originalPos;
    Vector3 destinationPos;
    float timer;

    void Start()
    {
        originalPos = PressurePlateObject.transform.position;
        destinationPos = PressurePlateObject.transform.position;
        timer = Time.deltaTime;
        objectsOnSwitch = new List<string>();
    }

    void Update()
    {
        PressurePlateObject.transform.position = Vector3.Lerp(originalPos, destinationPos, animationSpeed * timer);

        if (destinationPos == originalPos)
        {
            timer = Mathf.Clamp(timer - Time.deltaTime, 0.0f, 1.0f / animationSpeed);
        }
        else
        {
            timer = Mathf.Clamp(timer + Time.deltaTime, 0.0f, 1.0f / animationSpeed);
        }
    }

    // Lower Plate; Trigger Switch
    public void OnTriggerEnter(Collider other)
    {
        foreach (string s in WeightedObjectTag)
        {
            if ((other.tag == s) && !activated)
            {
                timer = Time.deltaTime;
                originalPos = PressurePlateObject.transform.position;
                destinationPos.y = -destinationPos.y;

                activated = true;
                ObjectToTrigger.SendMessage(TriggerFunctionCall);
                if(!objectsOnSwitch.Contains(other.tag))
                {
                    objectsOnSwitch.Add(other.tag);
                }
            }
        }
    }

    // Raise Plate; un-Trigger Switch (if allowed)
    public void OnTriggerExit(Collider other)
    {
        foreach (string s in WeightedObjectTag)
        {
            if ((other.tag == s) && allowDeactivate)
            {
                if (objectsOnSwitch.Contains(other.tag))
                {
                    objectsOnSwitch.Remove(other.tag);
                }
            }
        }

        if(objectsOnSwitch.Count == 0)
        {
            timer = Time.deltaTime;
            originalPos = PressurePlateObject.transform.position;
            destinationPos.y = -destinationPos.y;

            activated = false;
            ObjectToTrigger.SendMessage(TriggerFunctionCall);
        }
    }

}
