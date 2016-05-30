using UnityEngine;
using System.Collections;

public class UI_Manager : MonoBehaviour {

    public static UI_Manager instance = null;
    public Menu CurrentMenu;
    Menu PreviousMenu;
    public Menu Pause;
    public Menu Stats;
    public Menu LevelComplete;
    public Menu GameOverScreen;
    public float timePlaceHolder;


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
        if(CurrentMenu != null)
        {
            ShowMenu(CurrentMenu);
        }
       
	}

    //changes current ui menu?element
    public void ShowMenu(Menu menu)
    {
        if(menu != null)
        {
            if (CurrentMenu != null)
            {
                CurrentMenu.IsOpen = false;
                PreviousMenu = CurrentMenu;
            }

            CurrentMenu = menu;
            CurrentMenu.IsOpen = true;

        }
    }

    public void showPrevious()
    {
        if (PreviousMenu != null)
        {
            Menu temp = null;
            if (CurrentMenu != null)
            {
                CurrentMenu.IsOpen = false;
                temp = CurrentMenu;
            }

            CurrentMenu = PreviousMenu;
            PreviousMenu = temp;
            CurrentMenu.IsOpen = true;
        }
    }

	// Update is called once per frame
	void Update () {



        if ((Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.P)) && Game_Manager.instance.currentGameState == Game_Manager.GameState.PLAY && CurrentMenu != LevelComplete && CurrentMenu != GameOverScreen)
        {
            Game_Manager.instance.changeGameState(Game_Manager.GameState.PAUSE);
            UI_Manager.instance.ShowMenu(Pause);
            timePlaceHolder = Time.timeScale;
            Time.timeScale = 0;


        }
        else if (((Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.P)) && Game_Manager.instance.currentGameState == Game_Manager.GameState.PAUSE && CurrentMenu != LevelComplete && CurrentMenu != GameOverScreen))
        {
            Game_Manager.instance.changeGameState(Game_Manager.GameState.PLAY);
            UI_Manager.instance.ShowMenu(Stats);
            Time.timeScale = timePlaceHolder;
            
        }

        if(Game_Manager.instance.currentGameState == Game_Manager.GameState.PLAY)
        {
            if(Level_Manager.instance.LevelComplete())
            {
                Game_Manager.instance.currentGameState = Game_Manager.GameState.MENU;
                ShowMenu(LevelComplete);
            }
        }
	}

    public void GameOver()
    {
        //Character_Manager.instance.heal(100);
        ShowMenu(GameOverScreen);
    }


   
}
