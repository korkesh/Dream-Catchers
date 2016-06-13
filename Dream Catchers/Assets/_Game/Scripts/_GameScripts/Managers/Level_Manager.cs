using UnityEngine;
using System.Collections;


public class Level_Manager : MonoBehaviour {

    public static Level_Manager instance = null;

    //================================
    // Variables
    //================================

    public enum Levels
    {
        CLOWNDREAM
    }

    public string SceneName;
    public Levels LevelName;
    public Vector3 CheckPointPos;

    //-----------------
    // Defaults 
    //-----------------

    public Levels defaultLevel;
    public string defaultGameScene;
    public Vector3 defaultCheckPoint;

    //-----------------
    // Collectibles 
    //-----------------

    public int totalNumMemoryFrag;
    public int totalNumCollectibles;

    public int collectedMemoryFrag;
    public int collectedCollectibles;

    //================================
    // Methods
    //================================

    //-----------------
    // Initialization
    //-----------------

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
       // DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {

        ContinueLevel();

	}
	
	// Update is called once per frame
	void Update () {

	}

    /// <summary>
    /// Creates a new game save in player prefs
    /// </summary>
    public void NewGamePlayerPrefs()
    {

        PlayerPrefs.SetString("CurrentLevel", defaultLevel.ToString());
        PlayerPrefs.SetString("CurrentScene", defaultGameScene);
        PlayerPrefs.SetInt("TotalNumMemoryFrag", totalNumMemoryFrag);
        PlayerPrefs.SetInt("TotalNumCollectibles", totalNumCollectibles);
        PlayerPrefs.SetInt("LevelComplete", 0);
        PlayerPrefs.SetFloat("CheckPointX", defaultCheckPoint.x);
        PlayerPrefs.SetFloat("CheckPointY", defaultCheckPoint.y);
        PlayerPrefs.SetFloat("CheckPointZ", defaultCheckPoint.z);
        CheckPointPos = defaultCheckPoint;
    }

    //TODO: when checkpoints are added change this
    //public void Death()
    //{
    //    UI_Manager.instance.GameOver();
    //    //Game_Manager.instance.ChangeScene(defaultGameScene);
    //}

    public bool LevelComplete()
    {
        if(PlayerPrefs.GetInt("TotalNumMemoryFrag") == Character_Manager.instance.totalMemoryFragmentsCollected)
        {
            if(PlayerPrefs.GetInt("LevelComplete") == 0)
            {
                PlayerPrefs.SetInt("LevelComplete", 1);
                return true;
            }
        }
        return false;
    }

    public void newCheckPoint(Vector3 pos)
    {
        CheckPointPos = pos;
        PlayerPrefs.SetFloat("CheckPointX", CheckPointPos.x);
        PlayerPrefs.SetFloat("CheckPointY", CheckPointPos.y);
        PlayerPrefs.SetFloat("CheckPointZ", CheckPointPos.z);
    }

    public void ContinueLevel()
    {
        Game_Manager.instance.changeGameState(Game_Manager.GameState.PLAY);

        CheckPointPos.x = PlayerPrefs.GetFloat("CheckPointX");
        CheckPointPos.y = PlayerPrefs.GetFloat("CheckPointY");
        CheckPointPos.z = PlayerPrefs.GetFloat("CheckPointZ");

        if(CheckPointPos == Vector3.zero)
        {
            PlayerPrefs.SetFloat("CheckPointX", defaultCheckPoint.x);
            PlayerPrefs.SetFloat("CheckPointY", defaultCheckPoint.y);
            PlayerPrefs.SetFloat("CheckPointZ", defaultCheckPoint.z);
            CheckPointPos = defaultCheckPoint;
        }
    }
}
