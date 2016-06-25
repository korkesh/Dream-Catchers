///=====================================================================================
/// Author: Matt
/// Purpose: Adjusts characters speed in world swap when entering object
///======================================================================================

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ManipulationSpeed : ManipulationScript
{
    public GameObject player; // The player to act upon

    public float slowSpeed;
    public float increaseSpeed;

    public bool slowInNightmare;
    public bool slowInDream;

    public bool quickenInNightmare;
    public bool quickenInDream;

    private float playerOriginalWalkSpeed;
    private float playerOriginalRunSpeed;

    // Use this for initialization
    void Start()
    {
        currentManipType = MANIPULATION_TYPE.OTHER;

        PlayerMachine controllerScript = player.GetComponent<PlayerMachine>();
        playerOriginalWalkSpeed = controllerScript.WalkSpeed;
        playerOriginalRunSpeed = controllerScript.RunSpeed;


    }

    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;
    }

    // Use of trigger volumes to reduce and increase speed
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player && 
            ((slowInNightmare && currentObjectState == ManipulationManager.WORLD_STATE.NIGHTMARE)
            || (slowInDream && currentObjectState == ManipulationManager.WORLD_STATE.DREAM)))
        {
            PlayerMachine controllerScript = player.GetComponent<PlayerMachine>();
            controllerScript.WalkSpeed = slowSpeed;
            controllerScript.RunSpeed = slowSpeed;

        }
        else if (other.gameObject == player &&
            ((quickenInNightmare && currentObjectState == ManipulationManager.WORLD_STATE.NIGHTMARE)
            || (quickenInDream && currentObjectState == ManipulationManager.WORLD_STATE.DREAM)))
        {
            PlayerMachine controllerScript = player.GetComponent<PlayerMachine>();
            controllerScript.WalkSpeed = increaseSpeed;
            controllerScript.RunSpeed = increaseSpeed;

        }
        else
        {
            PlayerMachine controllerScript = player.GetComponent<PlayerMachine>();
            controllerScript.WalkSpeed = playerOriginalWalkSpeed;
            controllerScript.RunSpeed = playerOriginalRunSpeed;

        }
    }

    // Upon exit reset speed
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            PlayerMachine controllerScript = player.GetComponent<PlayerMachine>();
            controllerScript.WalkSpeed = playerOriginalWalkSpeed;
            controllerScript.RunSpeed = playerOriginalRunSpeed;
        }
    }
}
