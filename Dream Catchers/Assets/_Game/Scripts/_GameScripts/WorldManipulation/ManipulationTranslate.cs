///=====================================================================================
/// Author: Matt
/// Purpose: Translate and object between positions in world swap
///======================================================================================

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ManipulationTranslate : ManipulationScript
{ 
    public Transform objectTransform;

    public Vector3 translateDream;
    public Vector3 translateNightmare;

    public float translateDuration;

    public bool loop;
    public float timeBetweenLoop;
    public bool pauseInDream;
    public bool pauseInNightmare;

    Sequence mySequence;

    // Use this for initialization
    void Start()
    {
        // Set the default world state
        currentManipType = MANIPULATION_TYPE.TRANSLATE;
        
        objectTransform = gameObject.transform; // By default grabs the transform of the attached object
        
        if(loop)
        {
            // Create new Sequence object
            mySequence = DOTween.Sequence();

            // Pause at first position
            mySequence.Append(objectTransform.DOMove(translateDream, timeBetweenLoop));
            
            // Move To second position
            mySequence.Append(objectTransform.DOMove(translateNightmare, translateDuration));

            // Pause At second position
            mySequence.Append(objectTransform.DOMove(translateNightmare, timeBetweenLoop));

            // Move To first position
            mySequence.Append(objectTransform.DOMove(translateDream, translateDuration));

            // Loop sequence
            mySequence.SetLoops(-1, LoopType.Restart);
        }
    }

    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

        // Translate object to the given position over the given time duration
        if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM)
        {
            if (loop && pauseInDream)
            {
                mySequence.TogglePause();
            }
            else if (loop)
            {
                mySequence.TogglePause();
            }
            else
            {
                objectTransform.DOMove(translateDream, translateDuration);
            }
        }
        else
        {
            if (loop && pauseInNightmare)
            {
                mySequence.TogglePause();
            }
            else if (loop)
            {
                mySequence.TogglePause();
            }
            else
            {
                objectTransform.DOMove(translateNightmare, translateDuration);
            }
        }
    }
}
