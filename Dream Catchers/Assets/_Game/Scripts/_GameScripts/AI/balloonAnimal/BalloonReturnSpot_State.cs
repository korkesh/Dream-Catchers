using UnityEngine;
using System.Collections;

public class BalloonReturnSpot_State : BaseState {

    GameObject Player;
    public GameObject ReturnSpot;
    public NavMeshAgent NavAgent;
    public float PlayerInRange;
    public float MaxAllowableDist;

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
        else if (Vector3.Distance(Player.transform.position, transform.position) <= PlayerInRange && Vector3.Distance(Player.transform.position,ReturnSpot.transform.position) <= MaxAllowableDist)
        {
            fsm.changeState("Chase");

        }
        else if (Vector3.Distance(transform.position, ReturnSpot.transform.position) <= 0.05)
        {
            NavAgent.enabled = false;
        }
        else
        {
            NavAgent.SetDestination(ReturnSpot.transform.position);
        }
       

    }

    public override void Exit()
    {
        NavAgent.enabled = false;
    }

}
