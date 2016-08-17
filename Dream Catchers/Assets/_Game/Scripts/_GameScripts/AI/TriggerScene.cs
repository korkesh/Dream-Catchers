using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class TriggerScene : MonoBehaviour
{
    public enum TypeOfSwitch
    {
        AdditiveLoad,
        CompleteSwitch,
        Unload
    }

    public bool onEnter;
    public TypeOfSwitch enterSwitch;
    public string SceneName;
    public bool onExit;
    //public bool exitSwitch;
    public TypeOfSwitch exitSwitch;
    public bool saveScene;
    public bool LevelComplete;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && onEnter == true)
        {
            SceneChange(enterSwitch);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && onExit == true)
        {
            SceneChange(exitSwitch);
        }
    }

    public void SceneChange(TypeOfSwitch tos)
    {
        if (LevelComplete == true)
        {
            Level_Manager.Instance.LevelComplete(Level_Manager.Instance.CurrentLevel);
        }

        FadeScript fade = Camera.main.GetComponent<FadeScript>();
        if (fade == null)
        {
            SwitchScene(tos);
        }
        else
        {
            

            switch (tos)
            {
                case TypeOfSwitch.AdditiveLoad:
                    SwitchAdditive();
                    break;
                case TypeOfSwitch.CompleteSwitch:
                    fade.FadeOut();
                    Invoke("SwitchComplete", fade.fadeTime);
                    break;
                case TypeOfSwitch.Unload:
                    SwitchUnload();
                    break;
            }
        }

        if (saveScene == true)
        {
            PlayerPrefs.SetString("CurrentLevel", SceneName);
        }
    }

    // auto scene switch if fade was null
    void SwitchScene(TypeOfSwitch tos)
    {
        switch (tos)
        {
            case TypeOfSwitch.AdditiveLoad:
                SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
                break;
            case TypeOfSwitch.CompleteSwitch:
                SceneManager.LoadScene(SceneName);
                Level_Manager.Instance.checkPointContinue = false;
                setCurrentLevel();
                break;
            case TypeOfSwitch.Unload:
                SceneManager.UnloadScene(SceneName);
                break;
        }
    }

    void SwitchAdditive()
    {
        SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
    }

    void SwitchComplete()
    {
        SceneManager.LoadScene(SceneName);
        Level_Manager.Instance.checkPointContinue = false;
        setCurrentLevel();
    }

    void SwitchUnload()
    {
        SceneManager.UnloadScene(SceneName);
    }

    public void setCurrentLevel()
    {
        switch (SceneName)
        {
            case "HubWorld":
                {
                    Level_Manager.Instance.CurrentLevel = Level_Manager.Levels.HUB;
                    break;
                }
            case "Tutorial":
                {
                    Level_Manager.Instance.CurrentLevel = Level_Manager.Levels.TUTORIAL;
                    break;
                }
            case "Level1":
                {
                    Level_Manager.Instance.CurrentLevel = Level_Manager.Levels.CANNON;
                    break;
                }
            case "Level2":
                {
                    Level_Manager.Instance.CurrentLevel = Level_Manager.Levels.DAMAGE;
                    break;
                }
            case "Boss":
                {
                    Level_Manager.Instance.CurrentLevel = Level_Manager.Levels.BOSS;
                    break;
                }
            default:
                {
                    Level_Manager.Instance.CurrentLevel = Level_Manager.Levels.MENU;
                    break;
                }
        }

    }
}