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
        switch (tos)
        {
            case TypeOfSwitch.AdditiveLoad:
                SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
                break;
            case TypeOfSwitch.CompleteSwitch:
                SceneManager.LoadScene(SceneName);
                break;
            case TypeOfSwitch.Unload:
                SceneManager.UnloadScene(SceneName);
                break;
        }

        if(saveScene == true)
        {
            PlayerPrefs.SetString("CurrentLevel",SceneName);
        }
    }
}