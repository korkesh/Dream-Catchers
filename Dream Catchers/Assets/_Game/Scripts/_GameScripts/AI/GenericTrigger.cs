using UnityEngine;
using System.Collections;

public class GenericTrigger : MonoBehaviour {

    public GameObject ObjectToTrigger; // The object which will trigger upon activation
    public string TriggerFunctionCall; // Method to trigger

    public void OnTriggerEnter(Collider other)
    {
        ObjectToTrigger.SendMessage(TriggerFunctionCall);
    }
}
