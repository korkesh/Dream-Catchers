using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmallClownAI : MonoBehaviour {

    public List<GameObject> path;
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

        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            
            rigidB.useGravity = true;
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

    void Attack()
    {

    }

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

            rigidB.velocity = Jump(path[index].transform, JumpAngle);
        }
    }

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
}
