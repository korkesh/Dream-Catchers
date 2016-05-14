﻿using UnityEngine;
using System.Collections;


//temp class for testing dealing with items
public class Items : MonoBehaviour {

    public string itemName;
    
    //for now 0 is memory frag and 1 is other
    public int itemType;

    public string Scene;

    public Level_Manager.Levels Level;

    string key;

	// Use this for initialization
    void Awake()
    {

        //if the key exists in player prefsit checks to see if it is a 1 (meaning its been picked up), if so the object is deleted
	        key = Level.ToString() + Scene + itemName;
            if(PlayerPrefs.HasKey(key))
            {
                int pickedup = PlayerPrefs.GetInt(key);
                if(pickedup == 1)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                //object isnt in player prefs .. it is now added
                PlayerPrefs.SetInt(key, 0);
            }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //temp trigger stuff to pick up items and modify PlayerPrefs
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            if(itemType == 0)
            {
                Character_Manager.Instance.CollectOtherCollectible();
                
            }
            else
            {
                Character_Manager.Instance.CollectMemoryFrag();

            }
            PlayerPrefs.SetInt(key, 1);
            Destroy(gameObject);
        }
    }
}