using UnityEngine;
using System.Collections;

public class BalloonAttack_State : BaseState {

    //================================
    // Variables
    //================================

    public float radiusTolaunchAttack;
    public bool IsAttacking;
    bool IsJumping;
    public float waitTillNextAttack;
    bool wait;
    float waittime;
    public NavMeshAgent NavAgent;
    public float AttackDuration;
    float attackTIme;
    GameObject Player;
    public GameObject ReturnSpot;
    public float AllowableDistance;
    float speed;
    public float attackSpeed;
    public DamageDealer damage;
    public Rigidbody rigidB;
    public float shootAngle;


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
        IsAttacking = false;
        wait = false;
        waittime = waitTillNextAttack;
        attackTIme = AttackDuration;
        IsJumping = false;
        speed = NavAgent.speed;
        rigidB = GetComponent<Rigidbody>();
    }


    //-----------------
    // FSM Methods
    //-----------------

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

        }
        else if (Vector3.Distance(transform.position, ReturnSpot.transform.position) >= AllowableDistance && Vector3.Distance(ReturnSpot.transform.position, Player.transform.position) < AllowableDistance)
        {
            NavAgent.enabled = true;
            IsJumping = false;
            IsAttacking = false;
            NavAgent.SetDestination(ReturnSpot.transform.position);
        }
        //else if (wait == true)
        //{
        //    waittime -= Time.deltaTime;
        //    if(waittime <= 0)
        //    {
        //        wait = false;
        //        IsAttacking = true;
        //        waittime = waitTillNextAttack;
        //    }

        //}
        else if(IsAttacking == false)
        {

            if (Vector3.Distance(transform.position, Player.transform.position) <= radiusTolaunchAttack && Vector3.Distance(ReturnSpot.transform.position, Player.transform.position) < AllowableDistance)
            {
                IsAttacking = true;
            }
            else
            {
               // NavAgent.SetDestination(Player.transform.position);
                NavAgent.SetDestination(ReturnSpot.transform.position);
            }
        }
        else
        {
            if(IsJumping == false && Vector3.Distance(transform.position, Player.transform.position) >= (radiusTolaunchAttack-(radiusTolaunchAttack-2)))
            {
                NavAgent.enabled = false;
                rigidB.velocity = Jump(Player.transform, shootAngle);
                IsJumping = true;

            }
            else if (Vector3.Distance(transform.position, Player.transform.position) <= (radiusTolaunchAttack - (radiusTolaunchAttack - 2)) && rigidB.velocity.y == 0)
            {
                NavAgent.enabled = true;
                IsJumping = false;
                NavAgent.SetDestination(ReturnSpot.transform.position);

            }else if (rigidB.velocity.y == 0)
            {

                IsAttacking = false;
                IsJumping = false;
                NavAgent.enabled = true;

            }

            //attackTIme -= Time.deltaTime;
            //if(attackTIme >= 0)
            //{
            //    NavAgent.speed = attackSpeed;
            //    NavAgent.SetDestination(Player.transform.position);
            //}
            //else
            //{
            //    attackTIme = AttackDuration;
            //    wait = true;
            //    NavAgent.speed = speed;
            //    IsAttacking = false;
            //}
        }
    }

    public override void Exit()
    {
        NavAgent.enabled = false;
        wait = false;
        waittime = waitTillNextAttack;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            attackTIme = AttackDuration;
            wait = true;
            NavAgent.speed = speed;
            IsAttacking = false;
            damage.DealDamage();
             //&& wait == false
        }
    }

    Vector3 Jump( Transform target,  float angle)  
    {
        Vector3 dir = target.position - transform.position;  // get target direction
        float h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        float dist = dir.magnitude ;  // get horizontal distance
        float a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
        // calculate the velocity magnitude
        float sin = Mathf.Sin(2 * a);
        float div = dist * Physics.gravity.magnitude / sin;
        float vel = Mathf.Sqrt(div);
        Debug.Log(vel * dir.normalized);
        return vel * dir.normalized;
 }
}
