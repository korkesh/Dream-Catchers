///=====================================================================================
/// Author: Matt
/// Purpose: Scale an object between positions in world swap
///======================================================================================

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ManipulationScale : ManipulationScript {

    public Transform objectTransform;

    public Vector3 scaleDream;
    public Vector3 scaleNightmare;

    public float scaleDuration;

    // Use this for initialization
    void Start () {
        // Set the default world state
        currentManipType = MANIPULATION_TYPE.SCALE;

        objectTransform = gameObject.transform; // By default grabs the transform of the attached object
    }

    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

        // Scale object to the given position over the given time duration
        if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM)
        {
            objectTransform.DOScale(scaleDream, scaleDuration);
        }
        else
        {
            objectTransform.DOScale(scaleNightmare, scaleDuration);
        }
    }

   

}
