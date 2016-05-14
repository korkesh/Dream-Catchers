using UnityEngine;
using System.Collections;


public class Game_Manager : Singleton<Game_Manager> {

    //================================
    // Variables
    //================================

    //-----------------
    // State Variables
    //-----------------

    // World state enums 
    public enum WorldState 
    {
        DREAM,
        NIGHTMARE,
        NORMAL
    }

    // Game Flow states enums
    public enum GameState
    {
        INTRO,
        MENU,
        PLAY,
        PAUSE,
        GAMEOVER
    }
    
    // Current States
    public GameState currentGameState;
    public WorldState currentLevelState;


    //================================
    // Methods
    //================================

    //-----------------
    // Initialization
    //-----------------

    /// Initialize Game States
    void Awake()
    {  
        currentGameState = GameState.INTRO;
        currentLevelState = WorldState.NORMAL;
    }

    /// <summary>
    /// Deletes current playerprefs and gets level manager and 
    /// character manager to create new ones with default values
    /// </summary>
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        Level_Manager.Instance.NewGamePlayerPrefs();
        Character_Manager.Instance.NewGamePlayerPrefs();
    }

    //-----------------
    // Updates
    //-----------------

    /// Update is called once per frame
    void Update()
    {

    }

    //-----------------
    // State Handling
    //-----------------

    /// change game state
    public void changeGameState(GameState gs)
    {
        currentGameState = gs;
    }

    /// change level state
    public void changeWorldState(WorldState s)
    {
        currentLevelState = s;
    }

}
