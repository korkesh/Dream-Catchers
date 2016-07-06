///=====================================================================================
/// Author: Matt
/// Purpose: Creates bounce pads upon world change
///======================================================================================

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ManipulationBouncePad : ManipulationScript
{
    public GameObject player; // The player to act upon

    public float bounceMaxHeight;
    public float bounceMinHeight;
    public float bounceAcceleration;

    public bool bounceInDream;
    public bool bounceInNightmare;

    private bool bounceObject;
    private float originalMaxHeight;
    private float originalMinHeight;
    private float originalAcceleration;

    private float jumpReset = 0.5f;

    // Use this for initialization
    void Start()
    {
        // Set the default world state
        currentManipType = MANIPULATION_TYPE.OTHER;
        player = GameObject.FindGameObjectWithTag("Player");
        originalMaxHeight = player.GetComponent<PlayerMachine>().MaxJumpHeight;
        originalMinHeight = player.GetComponent<PlayerMachine>().MinJumpHeight;
        originalAcceleration = player.GetComponent<PlayerMachine>().JumpAcceleration;

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

    public void OnTriggerEnter(Collider other)
    {
        if (bounceObject && other.gameObject == player)
        {
            player.GetComponent<PlayerMachine>().MaxJumpHeight = bounceMaxHeight;
            player.GetComponent<PlayerMachine>().MinJumpHeight = bounceMinHeight;
            player.GetComponent<PlayerMachine>().JumpAcceleration = bounceAcceleration;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(bounceObject && other.gameObject == player)
        {
            player.GetComponent<PlayerMachine>().MaxJumpHeight = bounceMaxHeight;
            player.GetComponent<PlayerMachine>().MinJumpHeight = bounceMinHeight;
            player.GetComponent<PlayerMachine>().JumpAcceleration = bounceAcceleration;

            PlayerInputController inputScript = other.gameObject.GetComponent<PlayerInputController>();
            inputScript.toggleJump = true;

            StartCoroutine(ResetJump());
        }
    }

    public IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(jumpReset);

        player.GetComponent<PlayerMachine>().MaxJumpHeight = originalMaxHeight;
        player.GetComponent<PlayerMachine>().MinJumpHeight = originalMinHeight;
        player.GetComponent<PlayerMachine>().JumpAcceleration = originalAcceleration;
    }

    /*public void OnTriggerExit(Collider other)
    {
        if (bounceObject && other.gameObject == player)
        {
            player.GetComponent<PlayerMachine>().MaxJumpHeight = originalMaxHeight;
            player.GetComponent<PlayerMachine>().MinJumpHeight = originalMinHeight;
            player.GetComponent<PlayerMachine>().JumpAcceleration = originalAcceleration;
        }
    }*/

}
