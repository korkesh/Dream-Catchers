using UnityEngine;
using System.Collections;

public class TextBoxChangeGameState : MonoBehaviour {

    //script to change game states for text boxes
    public void PLayGame()
    {
        Game_Manager.instance.PlayGame();
    }

    public void Cinematic()
    {
        Game_Manager.instance.currentGameState = Game_Manager.GameState.CINEMATIC;
    }
}
