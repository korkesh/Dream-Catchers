using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


//temp script for changing the scene should be taken out later
public class TempSwitchScene : MonoBehaviour {

    public string NextScene;

	public void loadNext()
    {
        SceneManager.LoadScene(NextScene);
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
}
