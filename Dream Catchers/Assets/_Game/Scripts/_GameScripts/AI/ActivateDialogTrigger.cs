using UnityEngine;
using System.Collections;
using Fungus;

public class ActivateDialogTrigger : MonoBehaviour
{

    public Flowchart flC;
    public string message;

    void OnDestroy()
    {
        if(flC != null)
        {
            flC.SendFungusMessage(message);
        }
        
    }

    public void sendthemessage(string m)
    {
        if (flC != null)
        {
            flC.SendFungusMessage(m);
        }
    }
}
     
