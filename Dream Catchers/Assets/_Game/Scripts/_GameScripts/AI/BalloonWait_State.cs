using UnityEngine;
using System.Collections;

public class BalloonWait_State : BaseState {

    GameObject Player;
    public float waitTime;
    float wait;

    void Awake()
    {
        //get fsm
        fsm = this.gameObject.GetComponent<FSM>();
        Player = GameObject.FindGameObjectWithTag("Player");
        wait = waitTime;
    }

    public override void Enter()
    {
       
    }

    public override void Execute()
    {
        wait -= Time.deltaTime;
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            fsm.changeState("FloatToStart");

        }else if(wait <=  0)
        {
            fsm.changeState("Chase");
        }
        

    }

    public override void Exit()
    {
        wait = waitTime;
    }

}
