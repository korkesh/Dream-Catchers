//================================
// Alex
//  runs after player if close enough
//================================

using UnityEngine;
using System.Collections;

public class BalloonRun2 : BaseState {

    //================================
    // Variables
    //================================

    Animator anim;
    public float attackRadius;
    public float ChaseDist;
    GameObject Player;
    public NavMeshAgent NavAgent;

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
        anim = this.gameObject.GetComponentInChildren<Animator>();
        NavAgent = this.GetComponent<NavMeshAgent>();

    }


    //-----------------
    // Fsm methods
    //-----------------

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
        else if (Vector3.Distance(transform.position, Player.transform.position) <= attackRadius)
        {
            fsm.changeState("Attack");
        }
        else if (Vector3.Distance(transform.position, Player.transform.position) <= ChaseDist)
        {
            //set destination
            NavAgent.SetDestination(Player.transform.position);
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BalloonAnimalRun") == false)
            {
                anim.SetTrigger("Run");
                gameObject.SendMessage("Play", SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BalloonAnimalIdle") == false)
            {
                anim.SetTrigger("Idle");
            }
        }

        WatchPlayer();
    }

    public override void Exit()
    {
        NavAgent.enabled = false;
    }

    void WatchPlayer()
    {
        Vector3 lookDir = Player.transform.position;
        lookDir.y = transform.position.y;
        Vector3 targetDir = lookDir - transform.position;
        float step = 5 * Time.deltaTime;
        /////////
        Vector3 newDir = Vector3.RotateTowards(transform.right, targetDir, step, 0.0F);
        //transform.rotation = Quaternion.LookRotation(newDir);
        transform.right = newDir;
    }
}
