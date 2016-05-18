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
    }

    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

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
