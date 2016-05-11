using UnityEngine;
using System.Collections;

public class ManipulationManager : Singleton<ManipulationManager>
{
    public enum WORLD_STATE
    {
        DREAM = 0,
        NIGHTMARE = 1
    }
     
    public WORLD_STATE currentWorldState;

    // Use this for initialization
    void Start()
    {
        currentWorldState = WORLD_STATE.DREAM;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentWorldState = (currentWorldState == WORLD_STATE.DREAM) ? WORLD_STATE.NIGHTMARE : WORLD_STATE.DREAM;
        }

    }

}
