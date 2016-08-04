using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuControl : MonoBehaviour {

    public Button cont;
    public Button NewGame;
    public Button Options;
    public Button Credits;
    public Button Exit;
    public EventSystem ES;
    public float wait;
    float waitTime;

	// Use this for initialization
	void Start () {

        ES = GameObject.FindObjectOfType<EventSystem>();

        if(PlayerPrefs.GetFloat("Save") == 1)
        {
            cont.gameObject.SetActive(true);
            Navigation customNavCont = new Navigation();
            customNavCont.mode = Navigation.Mode.Explicit;
            customNavCont.selectOnDown = NewGame;
            customNavCont.selectOnUp = Exit;
            cont.navigation = customNavCont;
            Navigation customNavNew = new Navigation();
            customNavNew.mode = Navigation.Mode.Explicit;
            customNavNew.selectOnDown = Options;
            customNavNew.selectOnUp = cont;
            NewGame.navigation = customNavNew;
            Navigation customNavExit = new Navigation();
            customNavExit.mode = Navigation.Mode.Explicit;
            customNavExit.selectOnDown = cont;
            customNavExit.selectOnUp = Credits;
            Exit.navigation = customNavExit;
            ES.SetSelectedGameObject(cont.gameObject);
        }
        else
        {
            cont.gameObject.SetActive(false);
            if(ES != null)
            {
                ES.SetSelectedGameObject(NewGame.gameObject);
            }
            
        }

	}
	
	// Update is called once per frame
	void Update () {


	
	}

 
}
