//================================
// Alex
//  clown enemy behaviour
//================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmallClownAI : MonoBehaviour {

    //================================
    // Variables
    //================================

    public List<GameObject> path;
    public GameObject LaunchPoint;
    public GameObject Ball;
    public float JumpAngle;
    public float LuanchAngle;
    GameObject Player;
    public float DelayToAttack;
    float attackDelayTime;
    bool readyingAttack;
    public float DelayToMove;
    float moveDelayTime;
    public Rigidbody rigidB;
    int index;
    public float attackRadius;
    public bool loop;
    bool reverse;
    GameObject currentAttack;
    public float speedOfRotation;


    //================================
    // Methods
    //================================

    //-----------------
    // Initialization
    //-----------------

	// Use this for initialization
	void Awake () {

        Player = GameObject.FindGameObjectWithTag("Player");
        rigidB = GetComponent<Rigidbody>();
        moveDelayTime = DelayToMove;
        attackDelayTime = DelayToAttack;
        readyingAttack = true;
        index = 0;
        reverse = false;
	}
	
	// Update is called once per frame
	void Update () {
        //bock does nothing in dream
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            //enemy waits a bit throws a ball waits some more then jumps to new spot .. repeat
            
            rigidB.useGravity = true;
            WatchPlayer();
            if (readyingAttack == true)
            {
                if (rigidB.velocity.y == 0)
                {
                    attackDelayTime -= Time.deltaTime;
                    if (attackDelayTime <= 0)
                    {
                        if (Vector3.Distance(Player.transform.position, transform.position) <= attackRadius)
                        {
                            Attack();
                        }
                        readyingAttack = false;
                        attackDelayTime = DelayToAttack;
                    }
                }

            }
            else
            {
                moveDelayTime -= Time.deltaTime;
                if (moveDelayTime <= 0)
                {
                    move();
                    readyingAttack = true;
                    moveDelayTime = DelayToMove;
                }
            }
        }
        else
        {

            if(rigidB.detectCollisions == true && rigidB.velocity.y == 0)
            {
                rigidB.useGravity = false;
            }
        }

        
	
	}

    void OnDisable()
    {
        readyingAttack = true;
        moveDelayTime = DelayToMove;
        attackDelayTime = DelayToAttack;
    }


    //creates the ball
    void Attack()
    {
        GameObject rocket = (GameObject)Instantiate(Ball, LaunchPoint.transform.position, LaunchPoint.transform.rotation);
        currentAttack = rocket;
        ClownAttack cA = rocket.GetComponent<ClownAttack>();
        cA.clown = this.gameObject;
        cA.LaunchAngle = LuanchAngle;
        Rigidbody rocketClone = rocket.GetComponent<Rigidbody>();
        rocketClone.velocity = Jump(Player.transform.position, LuanchAngle,LaunchPoint.transform);
    }

    //destroys ball 
    void RemoveAttack()
    {
        if(currentAttack != null)
        {
            Destroy(currentAttack.gameObject);
        }
    }


    //movemt to next spot
    void move()
    {
        if(path.Count > 0)
        {
            if(loop == true)
            {
                index++;
                if(index >= path.Count)
                {
                    index = 0;
                }
            }
            else
            {
                if(reverse == true)
                {
                    index--;
                    if(index == 0)
                    {
                        reverse = false;

                    }
                }
                else
                {
                    index++;
                    if (index == path.Count-1)
                    {
                        reverse = true;
                    }
                }
            }

            rigidB.velocity = Jump(path[index].transform.position, JumpAngle, this.transform);
        }
    }


    //cannonball launch code again from unity answers
   public Vector3 Jump(Vector3 target, float angle, Transform current)
    {
        Vector3 dir = target - current.position;  // get target direction
        float h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        float dist = dir.magnitude;  // get horizontal distance
        float a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
        // calculate the velocity magnitude
        float sin = Mathf.Sin(2 * a);
        float div = dist * Physics.gravity.magnitude / sin;
       if (sin == 0 || div < 0)
       {
           return current.transform.forward * 2;
       }
        float vel = Mathf.Sqrt(div);
        return vel * dir.normalized;
    }

    //look at player
    void WatchPlayer()
    {
        Vector3 lookDir = Player.transform.position;
        lookDir.y = transform.position.y;
        Vector3 targetDir = lookDir- transform.position;
        float step = speedOfRotation * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
