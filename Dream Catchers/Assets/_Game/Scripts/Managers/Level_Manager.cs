using UnityEngine;
using System.Collections;
using System.Xml;

public class Level_Manager : MonoBehaviour {

    public static Level_Manager instance = null;
    public Game_Manager.State currentState;
    public string SceneName;

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
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //load info
    public void Load(XmlElement node)
    {

    }

    //save info
    public void Save(XmlElement node)
    {

    }
}
