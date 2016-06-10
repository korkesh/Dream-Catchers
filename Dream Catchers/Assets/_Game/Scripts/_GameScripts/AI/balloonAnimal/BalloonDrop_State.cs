using UnityEngine;
using System.Collections;

public class BalloonDrop_State : BaseState {

    Rigidbody rigidB;
    NavMeshAgent Navmesh;
    bool hit;

 
    void Awake()
    {
        //get fsm
        fsm = this.gameObject.GetComponent<FSM>();
        rigidB = this.gameObject.GetComponent<Rigidbody>();
        Navmesh = this.GetComponent<NavMeshAgent>();
    }


    public override void Enter()
    {
        rigidB.useGravity = true;
        rigidB.velocity = Vector3.zero;
        if(Navmesh != null)
        {
            Navmesh.enabled = false;
        }
        hit = false;
    }

    public override void Execute()
    {
       
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            fsm.changeState("FloatToStart");

        }
        else if (rigidB.velocity.y == 0 && hit == true)
        {
            fsm.changeState("Chase");
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
