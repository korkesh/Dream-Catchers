using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameStats : MonoBehaviour {

    //================================
    // Variables
    //================================

    public Image[] HealthBars;
    public float TimeofCollectibleUI;
    float timeCollUI;


    //-----------------
    // UI 
    //-----------------

    public Sprite FullHealthBar;
    public Sprite EmptyHealthBar;
    public Text SyncText;
    public Text MemFragNum;
    public Text CollectibleNum;
    public Sprite NUllImage;
    public Animator CollectibleAnim;

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
        CollectibleAnim.SetBool("IsOpen", false);
        timeCollUI = TimeofCollectibleUI;

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

            //MemFragNum.text = Character_Manager.instance.totalMemoryFragmentsCollected + " / " + Level_Manager.instance.totalNumMemoryFrag;
            if (CollectibleNum.text != Character_Manager.instance.totalCollectibles.ToString())
            {
                ShowCollect();
            }
            CollectibleNum.text = Character_Manager.instance.totalCollectibles.ToString();

        }else
        {
            displayed = false;
        }
	

        if(CollectibleAnim.GetBool("IsOpen") == true)
        {
            timeCollUI -= Time.deltaTime;
            if(timeCollUI <= 0)
            {
                CollectibleAnim.SetBool("IsOpen", false);
                timeCollUI = TimeofCollectibleUI;
            }
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

    public void ShowCollect()
    {
        if(CollectibleAnim.GetBool("IsOpen") == false)
        {
            CollectibleAnim.SetBool("IsOpen", true);
        }

        timeCollUI = TimeofCollectibleUI;
    }
}
