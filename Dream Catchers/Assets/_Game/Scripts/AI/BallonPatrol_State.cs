using UnityEngine;
using System.Collections;

public class BallonPatrol_State : BaseState {

    public Vector3 StartPos;
    public Vector3 StartPosForCalc;
   // public float startHeight;
    public float amplitudeofVerticalDisp;
    public float radiusOfCircle;
    public float speedVertical;
    public float speedHorizontal;
    float timeCounterVertical;
    float timeCounterHorizontal;


    void Awake()
    {
        //get fsm
        fsm = this.gameObject.GetComponent<FSM>();
        timeCounterVertical = 0;
        timeCounterHorizontal = 0;
        StartPos = this.transform.position + new Vector3 (radiusOfCircle,0,0);
        StartPosForCalc = this.transform.position;
    }


    public override void Enter()
    {

    }

    public override void Execute()
    {
        timeCounterHorizontal += Time.deltaTime * speedHorizontal;
        timeCounterVertical += Time.deltaTime * speedVertical;

        float x = StartPosForCalc.x + Mathf.Cos(timeCounterHorizontal) * radiusOfCircle;
        float y = StartPosForCalc.y + amplitudeofVerticalDisp * Mathf.Sin(timeCounterVertical);
        float z = StartPosForCalc.z + Mathf.Sin(timeCounterHorizontal) * radiusOfCircle;


        transform.position = new Vector3(x, y, z);

        if(ManipulationManager.Instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            fsm.changeState("WaitForPlayer");
        }

       

    }

    public override void Exit()
    {
        timeCounterVertical = 0;
        timeCounterHorizontal = 0;
    }
}
