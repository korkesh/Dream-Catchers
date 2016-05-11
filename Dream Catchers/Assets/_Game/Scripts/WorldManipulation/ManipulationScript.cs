using UnityEngine;
using System.Collections;

public class ManipulationScript : MonoBehaviour
{
    // The type of changes that can be applied by a state change
    public enum CHANGE_TYPE
    {
        NONE = 0,
        TEXTURE = 1,
        MESH = 2,
        ANIMATION = 3,
        PLATFORM = 4
    }

    // Objects state and change type
    public ManipulationManager.WORLD_STATE currentObjectState;
    public CHANGE_TYPE objectChangeType;

    // Textures
    public Texture dreamTexture;
    public Texture nightmareTexture;

    // Meshs
    public Mesh dreamMesh;
    public Mesh nightmareMesh;

    // Colliders
    public bool toggleCollisionWithMesh;
    public Collider dreamCollider;
    public Collider nightmareCollider;

    // Animations
    public bool toggleAnimationWithMesh;
    public AnimatorOverrideController dreamOverride;
    public AnimatorOverrideController nightmareOveride;

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
        if (ManipulationManager.Instance.currentWorldState != currentObjectState && objectChangeType != CHANGE_TYPE.NONE)
        {
            changeState(ManipulationManager.Instance.currentWorldState);
        }
    }

    // Applies the change logic to the current object upon state change
    void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;
        switch (objectChangeType)
        {
            // Texture Change
            case CHANGE_TYPE.TEXTURE:
                {
                    gameObject.GetComponent<Renderer>().material.mainTexture = (state == ManipulationManager.WORLD_STATE.DREAM) ? dreamTexture : nightmareTexture;
                    break;
                }
            case CHANGE_TYPE.MESH:
                {
                    gameObject.GetComponent<MeshFilter>().mesh = (state == ManipulationManager.WORLD_STATE.DREAM) ? dreamMesh : nightmareMesh;
                    if (toggleCollisionWithMesh)
                    {
                        if (state == ManipulationManager.WORLD_STATE.DREAM)
                        {
                            dreamCollider.enabled = true;
                            nightmareCollider.enabled = false;
                        }
                        else
                        {
                            nightmareCollider.enabled = true;
                            dreamCollider.enabled = false;
                        }
                    }
                    break;
                }
            case CHANGE_TYPE.ANIMATION:
                {
                    gameObject.GetComponent<Animator>().runtimeAnimatorController = (state == ManipulationManager.WORLD_STATE.DREAM) ? dreamOverride : nightmareOveride;
                    break;
                }
            case CHANGE_TYPE.PLATFORM:
                {
                    if (state == ManipulationManager.WORLD_STATE.DREAM)
                    {
                        dreamCollider.enabled = true;
                        gameObject.GetComponent<MeshFilter>().mesh = dreamMesh;

                    }
                    else
                    {
                        dreamCollider.enabled = false;
                        gameObject.GetComponent<MeshFilter>().mesh = null;
                    }

                    break;
                }
        }
    }
}
