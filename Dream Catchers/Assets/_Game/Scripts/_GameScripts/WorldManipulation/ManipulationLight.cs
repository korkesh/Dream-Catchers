///=====================================================================================
/// Author: Matt
/// Purpose: Handles changes in lighting upon world swap
///======================================================================================
using UnityEngine;
using System.Collections;

public class ManipulationLight : ManipulationScript
{
    public Color dreamColor;
    public Color nightmareColor;

    // Use this for initialization
    void Start()
    {
        // Set the default world state
        currentManipType = MANIPULATION_TYPE.OTHER;

    }

    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

        // Translate object to the given position over the given time duration
        if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM)
        {
            gameObject.GetComponent<Light>().color = dreamColor;
        }
        else
        {
            gameObject.GetComponent<Light>().color = nightmareColor;
        }
    }
}
