using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public Text numTickets;
    public Text LevelName;
    public Image MainFragment;
    public Image Fragment2;
    public Image Fragment3;

    public Sprite GreyGear;
    public Sprite GreyKey;
    public Sprite GreyLocket;
    public Sprite Gear;
    public Sprite Key;
    public Sprite Locket;

    public Menu menu;
    bool open;

	// Use this for initialization
	void Start () {

        open = false;
	}
	
	// Update is called once per frame
	void Update () {
	
        if(menu.IsOpen == true && open == false)
        {
            switch (Level_Manager.Instance.CurrentLevel)
            {
                case Level_Manager.Levels.HUB:

                    showAll();
                    break;

                case Level_Manager.Levels.TUTORIAL:

                    open = true;
                    LevelName.text = "Tutorial";
                    numTickets.text = PlayerPrefs.GetInt("collectedTutorialTickets").ToString() + " / " + PlayerPrefs.GetInt("totalTutorialTickets").ToString();
                    if (PlayerPrefs.GetInt("Fragment Gear") == 1)
                    {
                        MainFragment.sprite = Gear;
                        Fragment2.gameObject.SetActive(false);
                        Fragment3.gameObject.SetActive(false);
                    }
                    else
                    {
                        MainFragment.sprite = GreyGear;
                        Fragment2.gameObject.SetActive(false);
                        Fragment3.gameObject.SetActive(false);
                    }
                    break;

                case Level_Manager.Levels.CANNON:

                     open = true;
                     LevelName.text = "Cannons";
                     numTickets.text = PlayerPrefs.GetInt("collectedCannonTickets").ToString() + " / " + PlayerPrefs.GetInt("totalCannonTickets").ToString();
                     if (PlayerPrefs.GetInt("Fragment Key") == 1)
                    {
                        MainFragment.sprite = Key;
                        Fragment2.gameObject.SetActive(false);
                        Fragment3.gameObject.SetActive(false);
                    }
                    else
                    {
                        MainFragment.sprite = GreyKey;
                        Fragment2.gameObject.SetActive(false);
                        Fragment3.gameObject.SetActive(false);
                    }
                    break;

                case Level_Manager.Levels.DAMAGE:

                     open = true;
                     LevelName.text = "Damage Floors";
                     numTickets.text = PlayerPrefs.GetInt("collectedDamageTickets").ToString() + " / " + PlayerPrefs.GetInt("totalDamageTickets").ToString();
                     if (PlayerPrefs.GetInt("Fragment Locket") == 1)
                    {
                        MainFragment.sprite = Locket;
                        Fragment2.gameObject.SetActive(false);
                        Fragment3.gameObject.SetActive(false);
                    }
                    else
                    {
                        MainFragment.sprite = GreyLocket;
                        Fragment2.gameObject.SetActive(false);
                        Fragment3.gameObject.SetActive(false);
                    }
                    break;

                case Level_Manager.Levels.BOSS:

                    showAll();
                    break;

            }
        }
        else
        {
            open = false;
        }

	}


    public void showAll()
    {
        open = true;
        LevelName.text = "";
        numTickets.text = "X " + PlayerPrefs.GetInt("TotalOtherCollectiblesCollected").ToString();

        if (PlayerPrefs.GetInt("Fragment Gear") == 1)
        {
            Fragment2.gameObject.SetActive(true);
            Fragment2.sprite = Gear;
        }
        else
        {
            Fragment2.gameObject.SetActive(true);
            Fragment2.sprite = GreyGear;

        }

        if (PlayerPrefs.GetInt("Fragment Key") == 1)
        {
            MainFragment.sprite = Key;
        }
        else
        {
            MainFragment.sprite = GreyKey;

        }

        if (PlayerPrefs.GetInt("Fragment Locket") == 1)
        {
            Fragment3.gameObject.SetActive(true);
            Fragment3.sprite = Locket;
        }
        else
        {
            Fragment3.gameObject.SetActive(true);
            Fragment3.sprite = GreyLocket;

        }
    }
}
