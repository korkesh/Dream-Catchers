using UnityEngine;
using System.Collections;

public class Items : MonoBehaviour {

    //================================
    // Variables
    //================================

    //public string name;
    //for now 0 is memory frag and 1 is other
    public enum Type
    {
        FRAGEMENT,
        COLLECTIBLE,
        HEALTH_PICKUP
    }
    public Type type;
    public string Scene;
    public Level_Manager.Levels Level;
    string key;

    //================================
    // Methods
    //================================

    //-----------------
    // Initialization
    //-----------------
    void Awake()
    {

        //if the key exists in player prefsit checks to see if it is a 1 (meaning its been picked up), if so the object is deleted
	        key = Level.ToString() + Scene + gameObject.name.ToString();
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

    //-----------------
    // Updates
    //-----------------

	void Update () {
	
	}


    //-----------------
    // Trigger calls
    //-----------------

    // temp trigger stuff to pick up items and modify PlayerPrefs
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(type == Type.COLLECTIBLE)
            {
                Character_Manager.Instance.CollectOtherCollectible();
                
                
            }
            else if(type == Type.HEALTH_PICKUP)
            {
                Character_Manager.instance.heal(1);
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
