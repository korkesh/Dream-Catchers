using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ManipulationTranslate : ManipulationScript
{ 
    public Transform objectTransform;

    public Vector3 translateDream;
    public Vector3 translateNightmare;

    public float scaleDuration;

    // Use this for initialization
    void Start()
    {
        // Set the default world state
        currentManipType = MANIPULATION_TYPE.TRANSLATE;
    }

    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

        if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM)
        {
            objectTransform.DOMove(translateDream, scaleDuration);
        }
        else
        {
            objectTransform.DOMove(translateNightmare, scaleDuration);
        }
    }
}
