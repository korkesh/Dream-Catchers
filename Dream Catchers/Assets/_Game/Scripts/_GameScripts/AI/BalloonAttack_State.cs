using UnityEngine;
using System.Collections;

public class BalloonAttack_State : BaseState {

    public float radiusTolaunchAttack;
    public bool IsAttacking;
    public float waitTillNextAttack;
    bool wait;
    float waittime;
    public NavMeshAgent NavAgent;
    public float AttackDuration;
    float attackTIme;
    GameObject Player;
    GameObject ReturnSpot;
    float speed;
    public float attackSpeed;
    public DamageDealer damage;

    void Awake()
    {
        //get fsm
        fsm = this.gameObject.GetComponent<FSM>();
        Player = GameObject.FindGameObjectWithTag("Player");
        IsAttacking = false;
        wait = false;
        waittime = waitTillNextAttack;
        attackTIme = AttackDuration;
        speed = NavAgent.speed;
    }


    public override void Enter()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        NavAgent.enabled = true;
    }

    public override void Execute()
    {
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            fsm.changeState("FloatToStart");

        }else if (wait == true)
        {
            waittime -= Time.deltaTime;
            if(waittime <= 0)
            {
                wait = false;
                IsAttacking = true;
                waittime = waitTillNextAttack;
            }

        }else if(IsAttacking == false)
        {
           
            if(Vector3.Distance(transform.position,Player.transform.position) <= radiusTolaunchAttack)
            {
                IsAttacking = true;
            }
            else
            {
                NavAgent.SetDestination(Player.transform.position);
            }
        }
        else
        {
            attackTIme -= Time.deltaTime;
            if(attackTIme >= 0)
            {
                NavAgent.speed = attackSpeed;
                NavAgent.SetDestination(Player.transform.position);
            }
            else
            {
                attackTIme = AttackDuration;
                wait = true;
                NavAgent.speed = speed;
                IsAttacking = false;
            }
        }
    }

    public override void Exit()
    {
        NavAgent.enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && wait == false)
        {
            attackTIme = AttackDuration;
            wait = true;
            NavAgent.speed = speed;
            IsAttacking = false;
            damage.DealDamage();
        }
    }
}
