using UnityEngine;
using System.Collections;


public class Level_Manager : MonoBehaviour {

    public static Level_Manager instance = null;

    //================================
    // Variables
    //================================

    public enum Levels
    {
        CLOWNDREAM
    }

    public string SceneName;
    public Levels LevelName;

    //-----------------
    // Defaults 
    //-----------------

    public Levels defaultLevel;
    public string defaultGameScene;

    //-----------------
    // Collectibles 
    //-----------------

    public int totalNumMemoryFrag;
    public int totalNumCollectibles;

    public int collectedMemoryFrag;
    public int collectedCollectibles;

    //================================
    // Methods
    //================================

    //-----------------
    // Initialization
    //-----------------

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
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    /// <summary>
    /// Creates a new game save in player prefs
    /// </summary>
    public void NewGamePlayerPrefs()
    {
        PlayerPrefs.SetString("CurrentLevel", defaultLevel.ToString());
        PlayerPrefs.SetString("CurrentScene", defaultGameScene);
        PlayerPrefs.SetInt("TotalNumMemoryFrag", totalNumMemoryFrag);
        PlayerPrefs.SetInt("TotalNumCollectibles", totalNumCollectibles);
        PlayerPrefs.SetInt("LevelComplete", 0);
        
    }

    //TODO: when checkpoints are added change this
    //public void Death()
    //{
    //    UI_Manager.instance.GameOver();
    //    //Game_Manager.instance.ChangeScene(defaultGameScene);
    //}

    public bool LevelComplete()
    {
        if(PlayerPrefs.GetInt("TotalNumMemoryFrag") == Character_Manager.instance.totalMemoryFragmentsCollected)
        {
            if(PlayerPrefs.GetInt("LevelComplete") == 0)
            {
                PlayerPrefs.SetInt("LevelComplete", 1);
                return true;
            }
        }
        return false;
    }
}
