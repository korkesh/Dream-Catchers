//================================
// Alex
//  temp script for little into scene in new game
//================================
using UnityEngine;
using System.Collections;

public class TempMessageInto : MonoBehaviour {

    public string nextscene;
    public bool credits;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetButtonDown("Jump") == true)
        {
            Game_Manager.instance.ChangeScene(nextscene);
            if(credits)
            {
                Level_Manager.Instance.CurrentLevel = Level_Manager.Levels.MENU;
                Game_Manager.instance.currentGameState = Game_Manager.GameState.MENU;
            }
            else
            {
                Level_Manager.Instance.CurrentLevel = Level_Manager.Levels.TUTORIAL;
                PlayerPrefs.SetString("CurrentLevel", "Tutorial");
                Game_Manager.instance.currentGameState = Game_Manager.GameState.CINEMATIC;
            }
        }

	}
}
