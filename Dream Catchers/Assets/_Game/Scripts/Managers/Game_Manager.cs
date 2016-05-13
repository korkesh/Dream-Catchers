using UnityEngine;
using System.Collections;


public class Game_Manager : MonoBehaviour {

    public static Game_Manager instance = null;

    //state enums 
    public enum State 
    {
        DREAM,
        NIGHTMARE,
        NORMAL
    }

    //Game states enums
    public enum GameState
    {
        INTRO,
        MENU,
        PLAY,
        PAUSE,
        DEAD
    }

    public GameState currentGameState;
    public State currentLevelState;

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
        currentLevelState = State.NORMAL;
    }


    //change game state
    public void changeGameState(GameState gs)
    {
        currentGameState = gs;
    }

    //change level state
    public void changeLevelState(State s)
    {
        currentLevelState = s;
    }


	
	// Update is called once per frame
	void Update () {
	
	}

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        Level_Manager.instance.NewGamePlayerPrefs();
        Character_Manager.instance.NewGamePlayerPrefs();
    }


}
