using UnityEngine;
using System.Collections;

public class BalloonWaitForPlayer_State : BaseState {

    Rigidbody rigidB;
    public float attackRadius;
    GameObject Player;
    public NavMeshAgent NavAgent;



    void Awake()
    {
        //get fsm
        fsm = this.gameObject.GetComponent<FSM>();
        rigidB = this.gameObject.GetComponent<Rigidbody>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }


    public override void Enter()
    {
        rigidB.useGravity = true;
        Player = GameObject.FindGameObjectWithTag("Player");
        NavAgent.enabled = false;
    }

    public override void Execute()
    {
        Vector3 playerPos = Player.transform.position;
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            fsm.changeState("FloatToStart");

        }else if(Vector3.Distance(this.transform.position,playerPos) <= attackRadius)
        {
            if(rigidB.velocity.y == 0)
            {
                fsm.changeState("Attack");
            }
        }
    }

    public override void Exit()
    {
        NavAgent.enabled = false;
    }
}
