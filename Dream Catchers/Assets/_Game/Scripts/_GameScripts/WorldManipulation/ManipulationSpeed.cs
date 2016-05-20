using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ManipulationSpeed : ManipulationScript
{
    public GameObject player;

    public float slowSpeed;
    public float increaseSpeed;

    public bool slowInNightmare;
    public bool slowInDream;

    public bool quickenInNightmare;
    public bool quickenInDream;

    private float playerOriginalSpeed;

    // Use this for initialization
    void Start()
    {
        currentManipType = MANIPULATION_TYPE.OTHER;

        PlayerMachine controllerScript = player.GetComponent<PlayerMachine>();
        playerOriginalSpeed = controllerScript.WalkSpeed;

    }

    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player && 
            ((slowInNightmare && currentObjectState == ManipulationManager.WORLD_STATE.NIGHTMARE)
            || (slowInDream && currentObjectState == ManipulationManager.WORLD_STATE.DREAM)))
        {
            PlayerMachine controllerScript = player.GetComponent<PlayerMachine>();
            controllerScript.WalkSpeed = slowSpeed;
        }
        else if (other.gameObject == player &&
            ((quickenInNightmare && currentObjectState == ManipulationManager.WORLD_STATE.NIGHTMARE)
            || (quickenInDream && currentObjectState == ManipulationManager.WORLD_STATE.DREAM)))
        {
            PlayerMachine controllerScript = player.GetComponent<PlayerMachine>();
            controllerScript.WalkSpeed = increaseSpeed;
        }
        else
        {
            PlayerMachine controllerScript = player.GetComponent<PlayerMachine>();
            controllerScript.WalkSpeed = playerOriginalSpeed;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            PlayerMachine controllerScript = player.GetComponent<PlayerMachine>();
            controllerScript.WalkSpeed = playerOriginalSpeed;
        }
    }
}
