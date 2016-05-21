using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ManipulationBouncePad : ManipulationScript
{
    public GameObject player; // The player to act upon

    public bool bounceInDream;
    public bool bounceInNightmare;

    private bool bounceObject;

    // Use this for initialization
    void Start()
    {
        // Set the default world state
        currentManipType = MANIPULATION_TYPE.OTHER;

        setBounce();
    }

    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

        setBounce();
    }

    public void setBounce()
    {
        if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM && bounceInDream)
        {
            bounceObject = true;
        }
        else if (currentObjectState == ManipulationManager.WORLD_STATE.NIGHTMARE && bounceInNightmare)
        {
            bounceObject = true;
        }
        else
        {
            bounceObject = false;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(bounceObject && other.gameObject == player)
        {
            PlayerInputController inputScript = other.gameObject.GetComponent<PlayerInputController>();
            inputScript.toggleJump = true;
        }
    }

}
