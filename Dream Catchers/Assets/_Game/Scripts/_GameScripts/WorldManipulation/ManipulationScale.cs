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
    public bool isBoundry;
    public Collider boundry;

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
            if(gameObject.GetComponent<AudioSource>())
            {
                gameObject.SendMessage("Play");
            }
            objectTransform.DOScale(scaleDream, scaleDuration);
            if(isBoundry)
            {
                StartCoroutine(ToggleCollision());
            }
        }
        else
        {            
            if (gameObject.GetComponent<AudioSource>())
            {
                gameObject.SendMessage("PlayAlt");
            }
            objectTransform.DOScale(scaleNightmare, scaleDuration);
        }
    }

    public IEnumerator ToggleCollision()
    {
        boundry.enabled = !boundry.enabled;

        yield return new WaitForSeconds(0.8f);

        boundry.enabled = !boundry.enabled;

    }


}
