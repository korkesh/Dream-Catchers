using UnityEngine;
using System.Collections;
using System.Xml;

public class Character_Manager : MonoBehaviour {

    public static Character_Manager instance = null;

    public int currentHealth = 5;
    public int maxHealth = 5;
    public Game_Manager.State state  = Game_Manager.State.NORMAL;
    public Vector3 position;
    public bool invincible = false;

    public int totalMemoryFragmentsCollected;
    public int totalOtherCollectsCollected;




    //may not be needed depends what game object this is on or if it just referes to a specific script
    public GameObject Character;

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


    //take damage
    public void takeDamage(int damage)
    {
        if (currentHealth - damage <= 0)
        {
            //death state
            currentHealth = 0;
        }
        else
        {
            currentHealth -= damage;
        }
    }

    //heal
    public void heal(int health)
    {
        if (currentHealth + health >= maxHealth)
        {
            //full heal
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += health;
        }
    }

    public void increaseMaxHealth(int addedHealth)
    {
        maxHealth += addedHealth;
        currentHealth += addedHealth;
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
