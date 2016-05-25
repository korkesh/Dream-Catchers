using UnityEngine;
using System.Collections;

public class ManipulationScript : MonoBehaviour
{
    public enum MANIPULATION_TYPE
    {
        POP_IN = 0,
        SCALE = 1,
        TRANSLATE = 2,
        ROTATE = 3,
        OTHER = 4
    }

    // Objects state and change type
    public ManipulationManager.WORLD_STATE currentObjectState;
    protected MANIPULATION_TYPE currentManipType;

    // Use this for initialization
    void Start()
    {
        // Set the default world state
        currentObjectState = ManipulationManager.WORLD_STATE.DREAM;
    }

    // Update is called once per frame
    void Update()
    {
        // If the world state changes update object state and apply changes
        if (ManipulationManager.instance.currentWorldState != currentObjectState)
        {
            changeState(ManipulationManager.instance.currentWorldState);
        }
    }

    public virtual void changeState(ManipulationManager.WORLD_STATE state) { }
}
