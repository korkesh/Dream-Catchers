//================================
// Alex
//  Wait state for ballooon animal
//================================
using UnityEngine;
using System.Collections;

public class BalloonWait_State : BaseState {


    //================================
    // Variables
    //================================

    //-----------------
    // State Variables
    //-----------------

    public float shortwaitTime;
    public float longWaitTime;
    float wait;

    //================================
    // Methods
    //================================

    //-----------------
    // Initialization
    //-----------------

    void Awake()
    {
        //get fsm
        fsm = this.gameObject.GetComponent<FSM>();
        wait = shortwaitTime;
    }

    //-----------------
    // FSM Methods
    //-----------------

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
