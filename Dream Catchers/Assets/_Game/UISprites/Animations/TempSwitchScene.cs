using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
