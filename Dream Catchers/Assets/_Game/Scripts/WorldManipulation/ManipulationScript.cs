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

    // Manipulation Properties
    public bool isDreamPlatform = false;
    public bool isNightmarePlatform = false;

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
        if (ManipulationManager.Instance.currentWorldState != currentObjectState)
        {
            changeState(ManipulationManager.Instance.currentWorldState);
        }
    }

    // Applies the change logic to the current object upon state change
    void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

        changeTexture();
        changeMesh();
        changeCollider();

        togglePlatform();     
    }

    void changeTexture()
    {
        if (dreamTexture && nightmareTexture)
        {
            gameObject.GetComponent<Renderer>().material.mainTexture = (currentObjectState == ManipulationManager.WORLD_STATE.DREAM) ? dreamTexture : nightmareTexture;
        }
    }

    void changeMesh()
    {
        if (dreamMesh && nightmareMesh)
        {
            gameObject.GetComponent<MeshFilter>().mesh = (currentObjectState == ManipulationManager.WORLD_STATE.DREAM) ? dreamMesh : nightmareMesh;
        }
    }

    void changeCollider()
    {
        if (dreamCollider && nightmareCollider)
        {
            if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM)
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
    }

    void togglePlatform()
    {
        if(isDreamPlatform)
        {
            if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM)
            {
                dreamCollider.enabled = true;
                gameObject.GetComponent<MeshFilter>().mesh = dreamMesh;

            }
            else
            {
                dreamCollider.enabled = false;
                nightmareCollider.enabled = false;
                gameObject.GetComponent<MeshFilter>().mesh = null;
            }
        }
        else if (isNightmarePlatform)
        {
            if (currentObjectState == ManipulationManager.WORLD_STATE.NIGHTMARE)
            {
                nightmareCollider.enabled = true;
                gameObject.GetComponent<MeshFilter>().mesh = nightmareMesh;

            }
            else
            {
                dreamCollider.enabled = false;
                nightmareCollider.enabled = false;
                gameObject.GetComponent<MeshFilter>().mesh = null;
            }
        }
    }
}
