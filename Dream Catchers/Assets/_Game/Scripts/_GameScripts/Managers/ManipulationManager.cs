///=====================================================================================
/// Author: Matt
/// Purpose: Manages the World manipulation mechanic. Toggles state upon player input.
///          Handles BGM and skybox. 
///======================================================================================

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ManipulationManager : MonoBehaviour
{
    // Will live in Game Director
    public enum WORLD_STATE
    {
        DREAM = 0,
        NIGHTMARE = 1
    }
     
    // The current game state the world is in
    public WORLD_STATE currentWorldState;

    // Limit the ability to manipulate
    public float manipulationCooldown;
    bool onCooldown;
    public bool manipGained = false;

    // Skybox controls
    GameObject mainCamera;
    GameObject viewCamera;
    public Material skyboxDream;
    public Material skyboxNightmare;

    public static ManipulationManager instance = null;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        // DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        // Set default world state to dream
        currentWorldState = WORLD_STATE.DREAM;

        // Initialize DOTween
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);

    }

    // Update is called once per frame
    void Update()
    {
        // Toggles the World State upon player input
        if (Input.GetButtonDown("Fire3") && (Game_Manager.instance == null || Game_Manager.instance.isPlaying()) && !onCooldown && manipGained)
        {
            onCooldown = true;
            StartCoroutine(toggleCooldown());
            UI_Manager.instance.AnimateRecharge(manipulationCooldown);
            currentWorldState = (currentWorldState == WORLD_STATE.DREAM) ? WORLD_STATE.NIGHTMARE : WORLD_STATE.DREAM;

            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            viewCamera = GameObject.FindGameObjectWithTag("ViewCamera");

            if (currentWorldState == WORLD_STATE.DREAM)
            {
                mainCamera.GetComponent<Skybox>().material = skyboxDream;
                viewCamera.GetComponent<Skybox>().material = skyboxDream;

                // BGM
                Audio_Manager.Instance.NightmareBGM.mute = true;
                Audio_Manager.Instance.DreamBGM.mute = false;
            }
            else
            {
                mainCamera.GetComponent<Skybox>().material = skyboxNightmare;
                viewCamera.GetComponent<Skybox>().material = skyboxNightmare;

                // BGM
                Audio_Manager.Instance.DreamBGM.mute = true;
                Audio_Manager.Instance.NightmareBGM.mute = false;               
            }
        }

    }

    IEnumerator toggleCooldown()
    {
        yield return new WaitForSeconds(manipulationCooldown);

        // Code to execute after the delay
        onCooldown = false;
    }

    public void giveManip()
    {
        manipGained = true;
    }
}
