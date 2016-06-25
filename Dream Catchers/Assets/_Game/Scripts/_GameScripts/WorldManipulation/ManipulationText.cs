///=====================================================================================
/// Author: Matt
/// Purpose: Handles changes in text fonts upon world swap
///======================================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ManipulationText : ManipulationScript
{
    public Text text;
    public Font DreamFont;
    public Font NightMareFont;

    void Start()
    {
        // Set the default world state
        currentManipType = MANIPULATION_TYPE.OTHER;

    }

    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

        // Scale object to the given position over the given time duration
        if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM)
        {
            text.font = DreamFont;
        }
        else
        {
            text.font = NightMareFont;
        }
    }
}
