using UnityEngine;
using System.Collections;

public class GenericTrigger : MonoBehaviour {

<<<<<<< HEAD
    public GameObject ObjectToTrigger; // The object which will trigger upon activation
=======
    public GameObject ObjectToTrigger = null;
    public string ObjectToTriggerName; // The object which will trigger upon activation
>>>>>>> origin/LevelOneBuild
    public string TriggerFunctionCall; // Method to trigger

    public void OnTriggerEnter(Collider other)
    {
<<<<<<< HEAD
=======
        if (ObjectToTrigger == null)
        {
            ObjectToTrigger = GameObject.Find(ObjectToTriggerName);
        }

>>>>>>> origin/LevelOneBuild
        ObjectToTrigger.SendMessage(TriggerFunctionCall);
    }
}
