using UnityEngine;
using System.Collections;


//shows a menu
public class ShowMenu : MonoBehaviour {

    public string menuName;
    GameObject menu;
    bool found;


	// Use this for initialization
	void Awake () {

        found = false;
        menu = GameObject.Find(menuName);
        if(menu != null)
        {
            Menu m = menu.GetComponent<Menu>();
            if(m != null)
            {
                UI_Manager.instance.ShowMenu(m);
                found = true;
            }
           
        }

	}

    void Start()
    {
        if(found == false)
        {
            menu = GameObject.Find(menuName);
            if (menu != null)
            {
                Menu m = menu.GetComponent<Menu>();
                if (m != null)
                {
                    UI_Manager.instance.ShowMenu(m);
                    found = true;
                }

            }
        }
    }
	
}
