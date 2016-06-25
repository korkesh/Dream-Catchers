///=====================================================================================
/// Author: Matt
/// Purpose: Handles appearnce changes in objects upon world swap
///======================================================================================

using UnityEngine;
using System.Collections;

public class ManipulationPopIn : ManipulationScript {

    // Textures
    public Texture dreamTexture;
    public Texture nightmareTexture;

    // Meshs
    public Mesh dreamMesh;
    public Mesh nightmareMesh;

    // Colliders
    public Collider dreamCollider;
    public Collider nightmareCollider;

    // Materials
    public Material dreamMaterial;
    public Material nightmareMaterial;

    // Model
    public GameObject dreamModel;
    public GameObject nightmareModel;

    // Manipulation Properties
    public bool isDreamPlatform = false;
    public bool isNightmarePlatform = false;

    // Use this for initialization
    void Start()
    {
        // Set the default world state
        currentManipType = MANIPULATION_TYPE.POP_IN;
    }

    // Applies the change logic to the current object upon state change
    public override void changeState(ManipulationManager.WORLD_STATE state)
    {
        currentObjectState = state;

        changeTexture();
        changeMesh();
        changeCollider();
        changeMaterial();
        changeModel();

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

    void changeMaterial()
    {
        if (dreamMaterial && nightmareMaterial)
        {
            if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM)
            {
                gameObject.GetComponent<Renderer>().material = dreamMaterial;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material = nightmareMaterial;
            }
        }
    }

    void changeModel()
    {
        if (dreamModel && nightmareModel)
        {
            if (currentObjectState == ManipulationManager.WORLD_STATE.DREAM)
            {
                dreamModel.SetActive(true);
                nightmareModel.SetActive(false);
            }
            else
            {
                dreamModel.SetActive(false);
                nightmareModel.SetActive(true);
            }
        }
    }

    void togglePlatform()
    {
        if (isDreamPlatform)
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
