//================================
// Alex
//  balloon animal backup after attack
//================================
using UnityEngine;
using System.Collections;

public class BalloonBackup2 : BaseState {

    //================================
    // Variables
    //================================

    Animator anim;
    public float speed;
    public float backUp;
    Vector3 newPOs;
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
        anim = this.gameObject.GetComponentInChildren<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }


    //-----------------
    // FSM MEthods
    //-----------------

    public override void Enter()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("BalloonAnimalIdle") == false)
        {
            anim.SetTrigger("Idle");
        }

        newPOs = this.transform.position + (-1) * backUp * this.transform.forward;
    }

    public override void Execute()
    {
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            fsm.changeState("FloatToStart");

        }else if (Vector3.Distance(this.transform.position,newPOs) >= 0.2)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, newPOs, step);

        }
        else if (Vector3.Distance(this.transform.position, newPOs) < 0.2)
        {
            fsm.changeState("Run");
        }

        WatchPlayer();
    }

    public override void Exit()
    {

    }

    void WatchPlayer()
    {
        Vector3 lookDir = Player.transform.position;
        lookDir.y = transform.position.y;
        Vector3 targetDir = lookDir - transform.position;
        float step = 5* Time.deltaTime;
        /////
        Vector3 newDir = Vector3.RotateTowards(transform.right, targetDir, step, 0.0F);
        //transform.rotation = Quaternion.LookRotation(newDir);
        transform.right = newDir;
    }
}
