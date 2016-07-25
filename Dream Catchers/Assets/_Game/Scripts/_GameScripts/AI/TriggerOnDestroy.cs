using UnityEngine;
using System.Collections;

public class TriggerOnDestroy : MonoBehaviour {

    public GameObject ObjectToTrigger = null;
    public string ObjectToTriggerName; // The object which will trigger upon activation
    public string TriggerFunctionCall; // Method to trigger

    void OnDestroy()
    {
        if(ObjectToTrigger == null)
        {
            ObjectToTrigger = GameObject.Find(ObjectToTriggerName);
        }

        if (ObjectToTrigger != null)
        {
            //was getting errors about reciever missing when destroying scene.. added that u dont have to have one to hopefully fix problem
            ObjectToTrigger.SendMessage(TriggerFunctionCall,SendMessageOptions.DontRequireReceiver);
        }
        
    }
}
