//================================
// Alex
//  temp for switching scene in animation events and some buttons. still in use
//================================
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
        Game_Manager.instance.changeGameState(Game_Manager.GameState.MENU);
        //UI_Manager.instance.ShowMenu(MainMenu);
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
            Level_Manager.Instance.ContinueLevel();
            Character_Manager.instance.GoTocheckPoint();
        }
        else
        {
            newGame();
        }
    }

    public void ReloadLevel()
    {
        if (PlayerPrefs.GetString("CurrentLevel") == "Tutorial" || PlayerPrefs.HasKey("CurrentLevel") == false)
        {
            newGame();
        }
        else
        {
            Game_Manager.instance.ChangeScene(PlayerPrefs.GetString("CurrentLevel"));
            UI_Manager.instance.ShowMenu(GameObject.FindGameObjectWithTag("InGameUI").GetComponent<Menu>());
            //Game_Manager.instance.PlayGame();
            //I CAHNGES THIS

            Level_Manager.Instance.ContinueLevel();
            GameObject.FindGameObjectWithTag("InGameUI").GetComponent<InGameStats>().updateFragments();
            Character_Manager.instance.GoTocheckPoint();
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

    public void spawn()
    {

        Character_Manager.instance.GoTocheckPoint();
        Game_Manager.instance.changeGameState(Game_Manager.GameState.PLAY);
    }

    public void GamePlay()
    {
        Game_Manager.instance.PlayGame();
    }
}
