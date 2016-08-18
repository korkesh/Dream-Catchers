//================================
// Alex
//  items script. items delete themselves if they have been collected
//================================
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
    public GameObject sound;

    [HideInInspector]
    public string key;
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
        checkIfCollected();
        
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
                GameObject soundObject = Instantiate(sound, transform.position, transform.rotation) as GameObject;
                Destroy(soundObject, soundObject.GetComponent<AudioSource>().clip.length);

                Character_Manager.Instance.CollectOtherCollectible();
                Level_Manager.Instance.updateLevelTickets(this);
                
            }
            else if(type == Type.HEALTH_PICKUP)
            {
                Character_Manager.instance.heal(1);
            }
            else // Fragment
            {
                // Play celebration & turn off mesh/collider
                GameObject Player = GameObject.FindGameObjectWithTag("Player");
                Player.GetComponent<Animator>().SetTrigger("Celebrate");
                Player.GetComponent<PlayerMachine>().Celebrate();
                StartCoroutine(SceneChange());

                gameObject.GetComponent<MeshRenderer>().enabled = false;
                gameObject.GetComponent<Collider>().enabled = false;

                Character_Manager.Instance.CollectMemoryFrag();
                return;
            }

            //keeps health drops from not spawning
            if(type != Type.HEALTH_PICKUP)
            {
                PlayerPrefs.SetInt(key, 1);
            }

            if (GetComponent<PlaySound>())
            {
                PlaySound sound = GetComponent<PlaySound>();
                if (sound.correspondingSound)
                {
                    GameObject soundObj = Instantiate(sound.correspondingSound);
                    Destroy(soundObj, 1);
                }
            }

            Destroy(gameObject);
        }
    }


    public void checkIfCollected()
    {
        if (type == Type.FRAGEMENT)
        {
            key = gameObject.name.ToString();
            return;
        }


        if (PlayerPrefs.HasKey(key))
        {
            int pickedup = PlayerPrefs.GetInt(key);
            if (pickedup == 1)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (type != Type.HEALTH_PICKUP)
            {
                //object isnt in player prefs .. it is now added
                PlayerPrefs.SetInt(key, 0);
            }

        }
    }

    public IEnumerator SceneChange()
    {
        yield return new WaitForSeconds(3.0f);

        ManipulationManager.instance.currentWorldState = ManipulationManager.WORLD_STATE.DREAM;
        gameObject.GetComponent<TriggerScene>().SceneChange(gameObject.GetComponent<TriggerScene>().enterSwitch);
    }
}
