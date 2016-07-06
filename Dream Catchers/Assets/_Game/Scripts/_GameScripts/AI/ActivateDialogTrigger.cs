using UnityEngine;
using System.Collections;
using Fungus;

public class ActivateDialogTrigger : MonoBehaviour
{

    public Flowchart flC;
    public string message;

    void OnDestroy()
    {
        flC.SendFungusMessage(message);
    }
}
     
