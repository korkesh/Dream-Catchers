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


	// Use this for initialization
	void Awake () {

        Player = GameObject.FindGameObjectWithTag("Player");
        rigidB = GetComponent<Rigidbody>();
        moveDelayTime = DelayToMove;
        attackDelayTime = DelayToAttack;
        readyingAttack = true;
	}
	
	// Update is called once per frame
	void Update () {

        if(readyingAttack == true)
        {
            attackDelayTime -= Time.deltaTime;
            if(attackDelayTime <= 0)
            {
                Attack();
                readyingAttack = false;
            }
        }//else if()
	
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
}
