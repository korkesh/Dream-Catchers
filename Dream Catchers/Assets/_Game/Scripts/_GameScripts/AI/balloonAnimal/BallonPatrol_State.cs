//================================
// Alex
//  floating state for balloon animal
//================================

using UnityEngine;
using System.Collections;

public class BallonPatrol_State : BaseState {


    //================================
    // Variables
    //================================

    public Vector3 StartPos;
    public Vector3 StartPosForCalc;
   // public float startHeight;
    public float amplitudeofVerticalDisp;
    public float radiusOfCircle;
    public float speedVertical;
    public float speedHorizontal;
    float timeCounterVertical;
    float timeCounterHorizontal;

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
        timeCounterVertical = 0;
        timeCounterHorizontal = 0;
        StartPos = this.transform.position + new Vector3 (radiusOfCircle,0,0);
        StartPosForCalc = this.transform.position;
    }


    //-----------------
    // FSM Methods
    //-----------------

    public override void Enter()
    {

    }

    public override void Execute()
    {
        //calculates postition in air patrol
        timeCounterHorizontal += Time.deltaTime * speedHorizontal;
        timeCounterVertical += Time.deltaTime * speedVertical;

        float x = StartPosForCalc.x + Mathf.Cos(timeCounterHorizontal) * radiusOfCircle;
        float y = StartPosForCalc.y + amplitudeofVerticalDisp * Mathf.Sin(timeCounterVertical);
        float z = StartPosForCalc.z + Mathf.Sin(timeCounterHorizontal) * radiusOfCircle;


        transform.position = new Vector3(x, y, z);

        if(ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            fsm.changeState("Drop");
        }

       

    }

    public override void Exit()
    {
        //reset
        timeCounterVertical = 0;
        timeCounterHorizontal = 0;
    }
}
