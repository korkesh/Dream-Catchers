///=====================================================================================
/// Author: Matt
/// Purpose: Logic for multiple pressure plates that must be activated at the same time
///======================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiplePlatesController : MonoBehaviour {

    public List<GameObject> PressurePlateSwitches;

    public GameObject ObjectToTrigger;
    public string TriggerFunctionCall;

    public bool allowDeactivate;

    bool allActive = false;

    // Only activate if all switches toggled
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
