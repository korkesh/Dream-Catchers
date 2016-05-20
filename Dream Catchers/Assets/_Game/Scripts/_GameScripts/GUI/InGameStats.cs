using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameStats : MonoBehaviour {

    //================================
    // Variables
    //================================

    public Image[] HealthBars;

    //-----------------
    // UI 
    //-----------------

    public Sprite FullHealthBar;
    public Sprite EmptyHealthBar;
    public Text SyncText;
    public Text MemFragNum;
    public Text CollectibleNum;
    public Sprite NUllImage;

    //-----------------
    // Stats 
    //-----------------

    public int MaxHealth;
    public int CurrentHealth;
    public bool displayed = false;

    //================================
    // Methods
    //================================

    //-----------------
    // Initialization
    //-----------------

	void Start () {

        HealthBars = SyncText.GetComponentsInChildren<Image>();
        for(int i = 0; i < HealthBars.Length; i++)
        {
            HealthBars[i].sprite = NUllImage;
        }

        MaxHealth = Character_Manager.instance.maxHealth;
        CurrentHealth = Character_Manager.instance.currentHealth;
	}

    //-----------------
    // Update
    //-----------------

	void Update () {

        //if the game is in play mode check to see if anyvalues have changed. if so update ui
        if(UI_Manager.instance.CurrentMenu == this.GetComponent<Menu>())
        {
            if (displayed == false || MaxHealth != Character_Manager.instance.maxHealth || CurrentHealth != Character_Manager.instance.currentHealth)
            {
                healthChange();
                displayed = true;
            }

            MemFragNum.text = Character_Manager.instance.totalMemoryFragmentsCollected + " / " + Level_Manager.instance.totalNumMemoryFrag;
            CollectibleNum.text = Character_Manager.instance.totalCollectibles + " / " + Level_Manager.instance.totalNumCollectibles;

        }else
        {
            displayed = false;
        }
	
	}

    //-----------------
    // Stat Management
    //-----------------

    /// <summary>
    /// show proper health on UI
    /// </summary>

    public void healthChange()
    {
        MaxHealth = Character_Manager.instance.maxHealth;
        CurrentHealth = Character_Manager.instance.currentHealth;

        for(int i = 0; i < MaxHealth; i++)
        {
            if(i < CurrentHealth)
            {
                HealthBars[i].sprite = FullHealthBar;
            }
            else
            {
                HealthBars[i].sprite = EmptyHealthBar;
            }
        }

    }
}
