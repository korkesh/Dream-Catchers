using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSendMessage : MonoBehaviour {

    public GameObject Object;
    public List<string> messages;
    public bool deleteThis;

    //semd message on enter
    void OnTriggerEnter(Collider other)
    {
       if(Object != null)
       {
           for(int i = 0; i < messages.Count; i++)
           {
               Object.SendMessage(messages[i]);
           }

           if (deleteThis == true)
           {
               Destroy(this.gameObject);

           }
       }
    }
	
}
