using UnityEngine;
using System.Collections;


public class Character_Manager : MonoBehaviour {

    public static Character_Manager instance = null;

    public int currentHealth = 5;
    public int maxHealth = 5;
    public Game_Manager.State state  = Game_Manager.State.NORMAL;
    public Vector3 position;
    public bool invincible = false;

    public int totalMemoryFragmentsCollected;
    public int totalOtherCollectsCollected;

    public int newGameHealth = 5;
    public Vector3 newGamePosition;

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


    public void NewGamePlayerPrefs()
    {
        PlayerPrefs.SetInt("CurrentHealth", newGameHealth);
        PlayerPrefs.SetInt("MaxHealth", newGameHealth);
        PlayerPrefs.SetFloat("xPos", newGamePosition.x);
        PlayerPrefs.SetFloat("yPos", newGamePosition.y);
        PlayerPrefs.SetFloat("zPos", newGamePosition.z);
        PlayerPrefs.SetInt("TotalMemoryFragsCollected", 0);
        PlayerPrefs.SetInt("TotalOtherCollectiblesCollected", 0);
        totalMemoryFragmentsCollected = 0;
        totalOtherCollectsCollected = 0;
    }


    public void CollectMemoryFrag()
    {
        totalMemoryFragmentsCollected++;
        PlayerPrefs.SetInt("TotalMemoryFragsCollected", totalMemoryFragmentsCollected);
    }

    public void CollectOtherCollectible()
    {
        totalOtherCollectsCollected++;
        PlayerPrefs.SetInt("TotalOtherCollectiblesCollected", totalOtherCollectsCollected);
    }
}
