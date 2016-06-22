//================================
// Alex
//  Balloon drop 2
//================================

using UnityEngine;
using System.Collections;

public class BalloonDrop2 : BaseState {

    //================================
    // Variables
    //================================

    Animator anim;
    Rigidbody rigidB;
    NavMeshAgent Navmesh;
    bool hit;


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
        Navmesh = this.GetComponent<NavMeshAgent>();
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
        rigidB.useGravity = true;
        rigidB.velocity = Vector3.zero;
        if (Navmesh != null)
        {
            Navmesh.enabled = false;
        }
        hit = false;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("BalloonAnimalIdle") == false)
        {
            anim.SetTrigger("Idle");
        }
    }

    public override void Execute()
    {

        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            fsm.changeState("FloatToStart");

        }
        else if (rigidB.velocity.y == 0 && hit == true)
        {
            fsm.changeState("Run");
        }
    }

    public override void Exit()
    {
        if (Navmesh != null)
        {
            Navmesh.enabled = false;
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        hit = true;
    }
}
