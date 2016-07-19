using UnityEngine;
using System.Collections;

public class GenericTrigger : MonoBehaviour {

    public GameObject ObjectToTrigger = null;
    public string ObjectToTriggerName; // The object which will trigger upon activation

    public string TriggerFunctionCall; // Method to trigger

    public void OnTriggerEnter(Collider other)
    {


        if (ObjectToTrigger == null)
        {
            ObjectToTrigger = GameObject.Find(ObjectToTriggerName);
        }

        ObjectToTrigger.SendMessage(TriggerFunctionCall);
    }
}
