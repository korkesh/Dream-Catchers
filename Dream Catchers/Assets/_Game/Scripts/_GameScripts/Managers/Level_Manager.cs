using UnityEngine;
using System.Collections;


public class Level_Manager : Singleton<Level_Manager> {

    //================================
    // Variables
    //================================

    public enum Levels
    {
        CLOWNDREAM
    }

    public Game_Manager.WorldState currentState;
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
        
    }
   
}
