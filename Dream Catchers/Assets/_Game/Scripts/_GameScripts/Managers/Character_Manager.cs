using UnityEngine;
using System.Collections;


public class Character_Manager : Singleton<Character_Manager>
{
    public static Character_Manager instance = null;

    //================================
    // Variables
    //================================

    //may not be needed depends what game object this is on or if it just referes to a specific script
    public GameObject Character;

    //-----------------
    // Stats 
    //-----------------

    public int currentHealth = 5;
    public int maxHealth = 5;
    public int totalMemoryFragmentsCollected;
    public int totalCollectibles;

    //-----------------
    // Defaults 
    //-----------------

    public int newGameHealth = 5;
    public Vector3 newGamePosition;

    //-----------------
    // Debug 
    //-----------------

    public bool invincible = false;



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
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        totalMemoryFragmentsCollected = PlayerPrefs.GetInt("TotalMemoryFragsCollected");
        totalCollectibles = PlayerPrefs.GetInt("TotalOtherCollectiblesCollected"); ;
    }

    /// <summary>
    /// Creates a set of default player prefs upon selection of new game
    /// </summary>
    public void NewGamePlayerPrefs()
    {
        // Set Player Prefs
        PlayerPrefs.SetInt("CurrentHealth", newGameHealth);
        PlayerPrefs.SetInt("MaxHealth", newGameHealth);
        PlayerPrefs.SetFloat("xPos", newGamePosition.x);
        PlayerPrefs.SetFloat("yPos", newGamePosition.y);
        PlayerPrefs.SetFloat("zPos", newGamePosition.z);
        PlayerPrefs.SetInt("TotalMemoryFragsCollected", 0);
        PlayerPrefs.SetInt("TotalOtherCollectiblesCollected", 0);

        // Set variables
        totalMemoryFragmentsCollected = 0;
        totalCollectibles = 0;
        currentHealth = newGameHealth;
        maxHealth = newGameHealth;
    }

    //-----------------
    // Update
    //-----------------

    void Update()
    {

    }

    //-----------------
    // Stat Management
    //-----------------

    /// <summary>
    /// Deal damange to the player
    /// </summary>
    public void takeDamage(int damage)
    {
        currentHealth -= damage;

        // Restrict to Lower Bound of 0
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            //TODO: Handle Death state
            //Level_Manager.instance.Death();
            //Game_Manager.instance.changeGameState(Game_Manager.GameState.GAMEOVER);
            UI_Manager.instance.GameOver();
            heal(100);
            ManipulationManager.instance.currentWorldState = ManipulationManager.WORLD_STATE.DREAM;
            
        }

        PlayerPrefs.SetInt("CurrentHealth", currentHealth);
    }

    /// <summary>
    /// Heal Player
    /// </summary>
    public void heal(int health)
    {
        currentHealth += health;

        // Restrict to Upper Bound of "maxHealth"
        if (currentHealth > maxHealth)
        {
            //full heal
            currentHealth = maxHealth;
        }

        PlayerPrefs.SetInt("CurrentHealth", currentHealth);
    }

    /// <summary>
    /// Increase the characters max health
    /// </summary>
    public void increaseMaxHealth(int addedHealth)
    {
        maxHealth += addedHealth;
        currentHealth += addedHealth; // Adds boost in total health to current health

        PlayerPrefs.SetInt("MaxHealth", maxHealth);
        PlayerPrefs.SetInt("CurrentHealth", currentHealth);
    }

    //-----------------
    // Collectibles
    //-----------------

    /// <summary>
    /// Tracks when a memory fragment has been collected by the player; Set in Player Prefs
    /// </summary>
    public void CollectMemoryFrag()
    {
        totalMemoryFragmentsCollected += 1;
        PlayerPrefs.SetInt("TotalMemoryFragsCollected", totalMemoryFragmentsCollected);
    }

    /// <summary>
    /// Tracks when a level collectible has been collected by the player; Set in Player Prefs
    /// </summary>
    public void CollectOtherCollectible()
    {
        totalCollectibles += 1;
        PlayerPrefs.SetInt("TotalOtherCollectiblesCollected", totalCollectibles);
    }
}
