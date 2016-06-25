using UnityEngine;
using System.Collections;

public class BalloonFloatToStart2 : BaseState {
    //================================
    // Variables
    //================================

    Animator anim;
    Rigidbody rigidB;
    public Vector3 StartPos;
    public float speed;
    public float radiusOfCircle;



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
        rigidB = this.gameObject.GetComponent<Rigidbody>();
        StartPos = this.transform.position + new Vector3(radiusOfCircle, 0, 0);
        anim = this.gameObject.GetComponentInChildren<Animator>();
        if (anim == null)
        {
            Debug.Log("error no animator");
        }
    }

    //-----------------
    // FSM methods
    //-----------------

    public override void Enter()
    {
        rigidB.useGravity = false;
        rigidB.velocity = Vector3.zero;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("BalloonAnimalIdle") == false)
        {
            anim.SetTrigger("Idle");
        }
    }

    public override void Execute()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, StartPos, step);

        //state switches
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            fsm.changeState("Drop");

        }
        else if (Vector3.Distance(transform.position, StartPos) <= 0.09)
        {
            fsm.changeState("Patrol");
        }
    }

    public override void Exit()
    {

    }
}
