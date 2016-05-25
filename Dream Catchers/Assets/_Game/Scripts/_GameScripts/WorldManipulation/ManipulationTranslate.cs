using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ManipulationTranslate : ManipulationScript
{ 
    public Transform objectTransform;

    public Vector3 translateDream;
    public Vector3 translateNightmare;

    public float translateDuration;

    // Use this for initialization
    void Start()
    {
        // Set the default world state
        currentManipType = MANIPULATION_TYPE.TRANSLATE;

        objectTransform = gameObject.transform; // By default grabs the transform of the attached object
    }

    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

        // Translate object to the given position over the given time duration
        if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM)
        {
            objectTransform.DOMove(translateDream, translateDuration);
        }
        else
        {
            objectTransform.DOMove(translateNightmare, translateDuration);
        }
    }
}
