using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

    //-----------------
    // Main Menu
    //-----------------

    /// <summary>
    /// Start a new game from the first level (Scene to Load)
    /// </summary>
    public void StartNewGame(string sceneToLoad)
    {
        Game_Manager.instance.ChangeScene(sceneToLoad);
        UI_Manager.instance.ShowMenu(GameObject.FindGameObjectWithTag("InGameUI").GetComponent<Menu>());
        Game_Manager.instance.NewGame();
        GameObject.FindGameObjectWithTag("InGameUI").GetComponent<InGameStats>().updateFragments();
        Game_Manager.instance.PlayGame();
    }

    /// <summary>
    /// Continue most recent game
    /// </summary>
    public void ContinueGame(string sceneToLoad)
    {
        Game_Manager.instance.ChangeScene(sceneToLoad);
        UI_Manager.instance.ShowMenu(GameObject.FindGameObjectWithTag("InGameUI").GetComponent<Menu>());
        Game_Manager.instance.PlayGame();
        Level_Manager.instance.ContinueLevel();
        GameObject.FindGameObjectWithTag("InGameUI").GetComponent<InGameStats>().updateFragments();
    }

    /// <summary>
    /// Open Options panel
    /// </summary>
    public void OpenOptions()
    {
        UI_Manager.instance.ShowMenu(GameObject.FindGameObjectWithTag("Options").GetComponent<Menu>());
    }

    /// <summary>
    /// Exit Game
    /// </summary>
    public void ExitGame()
    {
        Game_Manager.instance.QuitGame();
    }

    /// <summary>
    /// Back Button
    /// </summary>
    public void Back()
    {
        UI_Manager.instance.showPrevious();
    }

    //-----------------
    // Pause Menu
    //-----------------

    public void UnPause()
    {
        Time.timeScale = UI_Manager.instance.timePlaceHolder;
    }

    public void Quit()
    {
        Game_Manager.instance.ChangeScene("MainMenu");
        UI_Manager.instance.ShowMenu(GameObject.FindGameObjectWithTag("MainMenu").GetComponent<Menu>());
        Game_Manager.instance.changeGameState(Game_Manager.GameState.MENU);
        UnPause();
    }

    public void ReturnToGame()
    {
        UnPause();
        Game_Manager.instance.PlayGame();
        UI_Manager.instance.ShowMenu(GameObject.FindGameObjectWithTag("InGameUI").GetComponent<Menu>());
    }
}
