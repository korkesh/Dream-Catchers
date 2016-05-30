using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


//temp script only text in into scene uses it right now
public class TempSwitchScene : MonoBehaviour {

    //================================
    // Variables
    //================================

    public string NextScene;
    public Menu MainMenu;
    public Menu InGameUI;


    //================================
    // Methods
    //================================

	public void loadNext()
    {
        SceneManager.LoadScene(NextScene);
        Game_Manager.instance.currentGameState = Game_Manager.GameState.MENU;
        UI_Manager.instance.ShowMenu(MainMenu);
    }

    public void newGame()
    {
        Game_Manager.instance.NewGame();
        SceneManager.LoadScene(NextScene);
    }


    //check if theres a game to load if not then it starts a new game
    public void load()
    {
        if(PlayerPrefs.HasKey("CurrentHealth"))
        {
            SceneManager.LoadScene(NextScene);
        }
        else
        {
            newGame();
        }
    }

    public void newMenu(Menu menu)
    {
        UI_Manager.instance.ShowMenu(menu);
    }


    public void InGameStats()
    {
        UI_Manager.instance.ShowMenu(InGameUI);
    }

    public void GMchangeGameState(Game_Manager.GameState gs)
    {
        Game_Manager.instance.changeGameState(gs);
    }
}
