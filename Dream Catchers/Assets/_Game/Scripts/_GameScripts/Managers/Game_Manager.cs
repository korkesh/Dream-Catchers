using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class Game_Manager : MonoBehaviour {

    public static Game_Manager instance = null;

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
        GAMEOVER,
        LEVELCOMPLETE
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
        Level_Manager.instance.NewGamePlayerPrefs();
        Character_Manager.instance.NewGamePlayerPrefs();
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

    //change state to play
    public void PlayGame()
    {
        changeGameState(GameState.PLAY);
    }


    //-----------------
    // Scene change
    //-----------------

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }


    public void quitGame()
    {
        Application.Quit();
    }
}
