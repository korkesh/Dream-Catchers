using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ManipulationManager : Singleton<ManipulationManager>
{
    // Will live in Game Director
    public enum WORLD_STATE
    {
        DREAM = 0,
        NIGHTMARE = 1
    }
     
    // The current game state the world is in
    public WORLD_STATE currentWorldState;

    // Use this for initialization
    void Start()
    {
        // Set default world state to dream
        // NOTE: Will not be set here by default, game director or level manager should handle this initialization
        currentWorldState = WORLD_STATE.DREAM;

        // Initialize DOTween
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
    }

    // Update is called once per frame
    void Update()
    {
        // Toggles the World State upon player input
        if (Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.Space))
        {
            currentWorldState = (currentWorldState == WORLD_STATE.DREAM) ? WORLD_STATE.NIGHTMARE : WORLD_STATE.DREAM;
        }

    }

}
