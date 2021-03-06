﻿//================================
// Alex
//  Attack state for balloon does jump and charge
//================================
using UnityEngine;
using System.Collections;

public class BalloonOneAttack_State : BaseState {

    //================================
    // Variables
    //================================

    GameObject Player;
    public GameObject ReturnSpot;
    public NavMeshAgent NavAgent;
    public float shootAngle;
    public float ChargeAttackDist;
    public float chargeTime;
    float time;
    float tempSpeed;
    public float chargeSpeed;
    Rigidbody rigidB;
    bool IsAttacking;
    public DamageDealer damage;


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
        Player = GameObject.FindGameObjectWithTag("Player");
        rigidB = GetComponent<Rigidbody>();
        damage = GetComponent<DamageDealer>();
        IsAttacking = false;
        time = chargeTime;
        tempSpeed = NavAgent.speed;
    }


    //-----------------
    // FSM MEthods
    //-----------------

    public override void Enter()
    {
        NavAgent.enabled = false;
    }

    public override void Execute()
    {
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            fsm.changeState("FloatToStart");

        }
        else if (IsAttacking == false && Vector3.Distance(transform.position, Player.transform.position) >= ChargeAttackDist)
        {
            // jump attack if in range
            rigidB.velocity = Jump(Player.transform, shootAngle);
            IsAttacking = true;

        }else if(IsAttacking == false)
        {
            //charge if too close to jump
            NavAgent.enabled = true;
            NavAgent.speed = chargeSpeed;
            NavAgent.SetDestination(Player.transform.position);
            time -= Time.deltaTime;
            if(time <= 0)
            {
                fsm.changeState("Wait");
            }

        }else if(rigidB.velocity.y == 0 && IsAttacking == true)
        {
            fsm.changeState("Wait");
        }

        //look at player
        WatchPlayer();

    }

    public override void Exit()
    {
        time = chargeTime;
        NavAgent.speed = tempSpeed;
        NavAgent.enabled = false;
        IsAttacking = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            damage.DealDamage();
            fsm.changeState("Wait");
            //&& wait == false
        }
    }


    //code for "jump/launch attack" .. math found online on unity answers for a cannonball
    Vector3 Jump(Transform target, float angle)
    {
        Vector3 dir = target.position - transform.position;  // get target direction
        float h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        float dist = dir.magnitude;  // get horizontal distance
        float a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
        // calculate the velocity magnitude
        float sin = Mathf.Sin(2 * a);
        float div = dist * Physics.gravity.magnitude / sin;
        float vel = Mathf.Sqrt(div);
        return vel * dir.normalized;
    }


    // looking at the player
    void WatchPlayer()
    {
        Vector3 lookDir = Player.transform.position;
        lookDir.y = transform.position.y;
        Vector3 targetDir = lookDir - transform.position;
        float step = 3 * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
