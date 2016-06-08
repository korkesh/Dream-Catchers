using UnityEngine;
using System.Collections;

public class BalloonWait_State : BaseState {

    GameObject Player;
    public float shortwaitTime;
    public float longWaitTime;
    float wait;

    void Awake()
    {
        //get fsm
        fsm = this.gameObject.GetComponent<FSM>();
        Player = GameObject.FindGameObjectWithTag("Player");
        wait = shortwaitTime;
    }

    public override void Enter()
    {
       if(Character_Manager.instance.invincible == true)
       {
           wait = longWaitTime;
       }
    }

    public override void Execute()
    {
        wait -= Time.deltaTime;
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            fsm.changeState("FloatToStart");

        }else if(wait <=  0 && Character_Manager.instance.invincible == false)
        {
            fsm.changeState("Chase");
        }
        

    }

    public override void Exit()
    {
        wait = shortwaitTime;
    }

}
