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
        DontDestroyOnLoad(gameObject);
    }

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
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
        {
            currentWorldState = (currentWorldState == WORLD_STATE.DREAM) ? WORLD_STATE.NIGHTMARE : WORLD_STATE.DREAM;
        }

    }

}
