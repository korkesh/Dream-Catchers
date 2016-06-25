//================================
// Alex
//  balloon animal idle float. Could be temporary just starting fresh with aniamtions
//================================
using UnityEngine;
using System.Collections;

public class BalloonIdle2 : BaseState {

    //================================
    // Variables
    //================================

    Animator anim;
    public Vector3 StartPos;
    public Vector3 StartPosForCalc;
    public float amplitudeofVerticalDisp;
    public float radiusOfCircle;
    public float speedVertical;
    public float speedHorizontal;
    float timeCounterVertical;
    float timeCounterHorizontal;
   // NavMeshAgent Navmesh;


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
        anim = this.gameObject.GetComponentInChildren<Animator>();
        if(anim == null)
        {
            Debug.Log("error no animator");
        }
        timeCounterVertical = 0;
        timeCounterHorizontal = 0;
        StartPos = this.transform.position + new Vector3(radiusOfCircle, 0, 0);
        StartPosForCalc = this.transform.position;
        //Navmesh = this.GetComponent<NavMeshAgent>();
    }


    //-----------------
    // FSM methods
    //-----------------

    public override void Enter()
    {
       // Navmesh.enabled = false;
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("BalloonAnimalIdle") == false)
        {
            anim.SetTrigger("Idle");
        }
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

        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
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
