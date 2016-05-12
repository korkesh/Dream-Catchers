using UnityEngine;
using System.Collections;

public class GameMaster : Singleton<GameMaster>
{
    //===============================
    // Fields
    //===============================

    public GameObject player; // TODO: get reference upon instantation instead of editor reference


	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(gameObject); // GM is only fully persistent object
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
