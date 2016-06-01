using UnityEngine;
using System.Collections;

public class BalloonChase_State : BaseState {

    public float attackRadius;
    public float MaxAllowedDist;
    GameObject Player;
    public GameObject ReturnSpot;
    public NavMeshAgent NavAgent;

    void Awake()
    {
        //get fsm
        fsm = this.gameObject.GetComponent<FSM>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }


    public override void Enter()
    {
        NavAgent.enabled = true;
    }

    public override void Execute()
    {
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            fsm.changeState("FloatToStart");

        }
        else if (Vector3.Distance(transform.position, ReturnSpot.transform.position) >= MaxAllowedDist && Vector3.Distance(Player.transform.position, ReturnSpot.transform.position) >= MaxAllowedDist)
        {
            //|| Vector3.Distance(ReturnSpot.transform.position, Player.transform.position) >= MaxAllowedDist
            fsm.changeState("ReturnSpot");
        }
        else if (Vector3.Distance(transform.position, Player.transform.position) <= attackRadius)
        {
            fsm.changeState("Attack");
        }
        else
        {
            NavAgent.SetDestination(Player.transform.position);
        }

    }

    public override void Exit()
    {
        NavAgent.enabled = false;
    }
}
