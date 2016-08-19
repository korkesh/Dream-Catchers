//================================
// Alex
//  Deals with all ui elements in play mode of game. Needs some rework once we decide how final ui should be
//================================
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
    public Image FragmentGear;
    public Image FragmentKey;
    public Image FragmentLocket;
    public Image HammerIcon;
    public int numFrag;
    Game_Manager.GameState state;
    ManipulationManager.WORLD_STATE Wstate;
    public Text textbox;
    bool showingText;
    float textTimer;
    public GameObject switcherUI;
    bool gainedSwitch;

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
        //updateFragments();
        state = Game_Manager.instance.currentGameState;
        gainedSwitch = false;
       
	}

    //-----------------
    // Update
    //-----------------

	void Update () {

        if(showingText == true)
        {
            textTimer -= Time.deltaTime;
            if(textTimer <= 0)
            {
                textbox.text = "";
                showingText = false;
            }
        }

        if(ManipulationManager.instance.manipGained == true && gainedSwitch == false)
        {
            if(switcherUI != null)
            {
                switcherUI.SetActive(true);
                gainedSwitch = true;
            }

        }
        else if (switcherUI != null && ManipulationManager.instance.manipGained == false && gainedSwitch == true)
        {
            switcherUI.SetActive(false);
            gainedSwitch = false;
        }
        //if (Game_Manager.instance.currentGameState == Game_Manager.GameState.PAUSE)
        //{
        //    //ShowCollect();
        //}

        //if (state != Game_Manager.instance.currentGameState && Game_Manager.instance.currentGameState == Game_Manager.GameState.PLAY)
        //{
        //    updateFragments();
        //}

        //if(Wstate != ManipulationManager.instance.currentWorldState)
        //{
        //    hammerChange();
        //}

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

            //if(numFrag != Character_Manager.instance.totalMemoryFragmentsCollected)
            //{
            //    updateFragments();
            //}

        }else
        {
            displayed = false;
        }

        if (CollectibleAnim.GetBool("IsOpen") == true )
        {
            timeCollUI -= Time.deltaTime;
            if(timeCollUI <= 0)
            {
                CollectibleAnim.SetBool("IsOpen", false);
                timeCollUI = TimeofCollectibleUI;
            }
        }

        state = Game_Manager.instance.currentGameState;
        Wstate = ManipulationManager.instance.currentWorldState;
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

    public void updateFragments()
    {
        //if (PlayerPrefs.HasKey("Fragment Gear") == true)
        //{
        //    if (PlayerPrefs.GetInt("Fragment Gear") == 1)
        //    {
        //        FragmentGear.sprite = FragmentGearPic;
        //    }
        //    else
        //    {
        //        FragmentGear.sprite = DimFragmentGearPic;
        //    }
        //}
        //else
        //{
        //    FragmentGear.sprite = DimFragmentGearPic;
        //}

        //if (PlayerPrefs.HasKey("Fragment Key") == true)
        //{
        //    if (PlayerPrefs.GetInt("Fragment Key") == 1)
        //    {
        //        FragmentKey.sprite = FragmentKeyPic;
        //    }
        //    else
        //    {
        //        FragmentKey.sprite = DimFragmentKeyPic;
        //    }
        //}
        //else
        //{
        //    FragmentKey.sprite = DimFragmentKeyPic;
        //}

        //if (PlayerPrefs.HasKey("Fragment Locket") == true)
        //{
        //    if (PlayerPrefs.GetInt("Fragment Locket") == 1)
        //    {
        //        FragmentLocket.sprite = FragmentLocketPic;
        //    }
        //    else
        //    {
        //        FragmentLocket.sprite = DimFragmentLocketPic;
        //    }
        //}
        //else
        //{
        //    FragmentLocket.sprite = DimFragmentLocketPic;
        //}

        //numFrag = Character_Manager.instance.totalMemoryFragmentsCollected;
    }

    void hammerChange()
    {
        //if(ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        //{
        //    HammerIcon.sprite = DreamHammer;

        //}
        //else
        //{
        //    HammerIcon.sprite = NightHammer;
        //}
    }


    public void ShowTextInBox(string s, float time)
    {
        textbox.text = s;
        textTimer = time;
        showingText = true;
    }
}
