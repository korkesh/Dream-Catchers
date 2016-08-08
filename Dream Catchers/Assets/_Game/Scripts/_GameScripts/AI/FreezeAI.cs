using UnityEngine;
using System.Collections;

public class FreezeAI : MonoBehaviour {

    public FSM AIFSM;
    public Collider enemyCollider;

	// Use this for initialization
	void Awake () {
	
        if(AIFSM != null)
        {
            if(AIFSM == null)
            {
                AIFSM = this.GetComponent<FSM>();
                
            }

            if(enemyCollider == null)
            {
                enemyCollider = this.GetComponent<Collider>();
            }

            if(Game_Manager.instance.currentGameState != Game_Manager.GameState.PLAY)
            {
                AIFSM.enabled = false;
                enemyCollider.enabled = false;
            }
        }

	}
	
	// Update is called once per frame
	void Update () {

        if((Game_Manager.instance.currentGameState != Game_Manager.GameState.PLAY) && AIFSM.enabled == true)
        {
             AIFSM.enabled = false;
             enemyCollider.enabled = false;

        }
        else if ((Game_Manager.instance.currentGameState == Game_Manager.GameState.PLAY) && AIFSM.enabled == false)
        {
            AIFSM.enabled = true;
            enemyCollider.enabled = true;
        }
	
	}
}
