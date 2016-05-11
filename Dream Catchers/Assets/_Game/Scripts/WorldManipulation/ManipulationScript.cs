using UnityEngine;
using System.Collections;

public class ManipulationScript : MonoBehaviour
{

    public enum CHANGE_TYPE
    {
        NONE = 0,
        TEXTURE = 1,
        MESH = 2,
        ANIMATION = 3,
        PLATFORM = 4
    }

    public ManipulationManager.WORLD_STATE currentObjectState;
    public CHANGE_TYPE objectChangeType;

    public Texture dreamTexture;
    public Texture nightmareTexture;

    public Mesh dreamMesh;
    public Mesh nightmareMesh;

    public bool toggleCollisionWithMesh;
    public Collider dreamCollider;
    public Collider nightmareCollider;

    public bool toggleAnimationWithMesh;
    public AnimatorOverrideController dreamOverride;
    public AnimatorOverrideController nightmareOveride;

    // Use this for initialization
    void Start()
    {
        currentObjectState = ManipulationManager.WORLD_STATE.DREAM;
    }

    // Update is called once per frame
    void Update()
    {

        if (ManipulationManager.Instance.currentWorldState != currentObjectState && objectChangeType != CHANGE_TYPE.NONE)
        {
            changeState(ManipulationManager.Instance.currentWorldState);
        }
    }

    void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;
        switch (objectChangeType)
        {
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
                        gameObject.GetComponent<ParticleSystem>().Stop();
                        gameObject.GetComponent<MeshFilter>().mesh = dreamMesh;

                    }
                    else
                    {
                        dreamCollider.enabled = false;
                        gameObject.GetComponent<ParticleSystem>().Play();
                        gameObject.GetComponent<MeshFilter>().mesh = null;
                    }

                    break;
                }
        }
    }
}
