using UnityEngine;
using System.Collections;

public class correctLevel : MonoBehaviour {

    public Level_Manager.Levels level;
    public bool play;

	// tells what level it is
	void Start () {

        Level_Manager.Instance.CurrentLevel = level;
        if(play == true)
        {
            Game_Manager.instance.currentGameState = Game_Manager.GameState.PLAY;
        }

	}

}
