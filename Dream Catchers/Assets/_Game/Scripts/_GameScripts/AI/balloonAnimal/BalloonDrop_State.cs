using UnityEngine;
using System.Collections;

public class BalloonDrop_State : BaseState {

    Rigidbody rigidB;
 
    void Awake()
    {
        //get fsm
        fsm = this.gameObject.GetComponent<FSM>();
        rigidB = this.gameObject.GetComponent<Rigidbody>();
    }


    public override void Enter()
    {
        rigidB.useGravity = true;
        
    }

    public override void Execute()
    {
       
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            fsm.changeState("FloatToStart");

        }
        else if (rigidB.velocity.y == 0)
        {
            fsm.changeState("Chase");
        }
    }

    public override void Exit()
    {
        
    }
}
