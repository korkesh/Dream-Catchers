using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiplePlatesController : MonoBehaviour {

    public List<GameObject> PressurePlateSwitches;

    public GameObject ObjectToTrigger;
    public string TriggerFunctionCall;

    public bool allowDeactivate;

    bool allActive = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CheckSwitches()
    {
        foreach (GameObject go in PressurePlateSwitches)
        {
            if(go.GetComponent<PressurePlate>().activated == false)
            {
                if(allActive && allowDeactivate)
                {
                    ObjectToTrigger.SendMessage(TriggerFunctionCall);
                }

                allActive = false;
                return;
            }
        }

        allActive = true;
        ObjectToTrigger.SendMessage(TriggerFunctionCall);
    }
}
