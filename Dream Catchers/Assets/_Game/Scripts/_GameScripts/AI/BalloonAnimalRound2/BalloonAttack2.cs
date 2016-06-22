//================================
// Alex
//  play Attackanim
//================================
using UnityEngine;
using System.Collections;

public class BalloonAttack2 : BaseState {

    //================================
    // Variables
    //================================

    Animator anim;
    public DamageDealer damage;
    GameObject Player;


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
        damage = GetComponent<DamageDealer>();
        anim = this.gameObject.GetComponentInChildren<Animator>();
        if (anim == null)
        {
            Debug.Log("error no animator");
        }
    }


    //-----------------
    // FSM MEthods
    //-----------------

    public override void Enter()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("BalloonAnimalAttack") == false)
        {
            anim.SetTrigger("Attack");
        }
    }

    public override void Execute()
    {
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            fsm.changeState("FloatToStart");

        }else if(anim.GetCurrentAnimatorStateInfo(0).IsName("BalloonAnimalIdle") == true)
        {
            fsm.changeState("BackUp");
        }
       
        //look at player
        WatchPlayer();

    }

    public override void Exit()
    {
       
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            damage.DealDamage();
            fsm.changeState("BackUp");
        }
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
