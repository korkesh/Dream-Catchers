using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HandScript : MonoBehaviour {

    public int Health;
    public BallSpawner spawner;
    public GameObject startSwipePos;
    public GameObject endSwipePos;
    public GameObject startSmackPos;
    public GameObject hunter;
    public Vector3 rotationSwipe;
    public Vector3 rotationSmack;
    public float durationFirstSectionSwipe;
    public float durationSecondSectionSwipe;
    public float durationThirdSectionSwipe;
    public float durationFirstSectionSmack;
    public float durationSecondSectionSmack;
    public float durationThirdSectionSmack;
    Vector3 tempPos;
    public Vector3 tempRot;
    public bool isAttacking;
    public DamageDealer Damage;

	// Use this for initialization
	void Start () {
        hunter = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        //for testing
        if(isAttacking == true)
        {
            Swipe();
            isAttacking = false;
        }
        
	}


    public void ThrowBall()
    {
        spawner.Spawn();
    }

    public void Swipe()
    {
        SwipeSequence();
    }

    public void SwipeSequence()
    {
        //TweenCallback CallBack;
        Sequence swipeSequence = DOTween.Sequence();
        tempPos = this.transform.position;
        tempRot = this.transform.rotation.eulerAngles;
        swipeSequence.Append(transform.DOMove(startSwipePos.transform.position, durationFirstSectionSwipe, false)).Append(transform.DOMove(endSwipePos.transform.position, durationSecondSectionSwipe, false)).Append(transform.DOMove(tempPos, durationThirdSectionSwipe, false));
        swipeSequence.Insert(0, transform.DORotate(rotationSwipe, 0.5f));
        swipeSequence.Insert(durationFirstSectionSwipe + durationSecondSectionSwipe, transform.DORotate(tempRot, 0.5f));
    }

    public void SmackDown()
    {
        Sequence SmackDown = DOTween.Sequence();
        tempPos = this.transform.position;
        tempRot = this.transform.rotation.eulerAngles;
        SmackDown.Append(transform.DOMove(startSmackPos.transform.position, durationFirstSectionSmack, false)).Append(transform.DOMove(hunter.transform.position, durationSecondSectionSmack, false)).Append(transform.DOMove(tempPos, durationThirdSectionSmack, false));
        SmackDown.Insert(durationFirstSectionSmack, transform.DORotate(rotationSmack, durationSecondSectionSmack));
        SmackDown.Insert(durationFirstSectionSwipe + durationSecondSectionSwipe, transform.DORotate(tempRot, 0.5f));
    }

    void OnDestroy()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit " + other.gameObject.name);
       if(other.gameObject.tag == "Player")
       {
           Debug.Log("hunter");
           //isAttacking == true && 
           Damage.DealDamage();
       }
    }


}
