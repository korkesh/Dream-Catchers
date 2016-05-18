using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ManipulationScale : ManipulationScript {

    public Transform objectTransform;

    // Use this for initialization
    void Start () {
        // Set the default world state
        currentManipType = MANIPULATION_TYPE.SCALE;
    }

    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

        objectTransform.DOScale(new Vector3(2.0f, 1.0f, 1.0f), 1.0f);
    }

}
