using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HandScript : MonoBehaviour {

    public enum Mode
    {
        THROW,
        SWIPE,
        SMACK,
        HOVER,
        BLOCK,
        CHARGE,
        NONE
    }
   public  Mode attack;

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
    public float durationForthSectionSwipe;
    public float SwipeWait;
    public float durationFirstSectionChargeSmack;
    public float durationSecondSectionChargeSmack;
    public float durationFirstSectionSmack;
    public float durationSecondSectionSmack;
    public float HoverY;
    public float aboveGroound;
    Vector3 tempPos;
    public Vector3 tempRot;
    public bool isAttacking;
    public DamageDealer Damage;
    public float smackFollow;
    float smackFollowTime;
    Vector3 follow;

	// Use this for initialization
	void Start () {
        hunter = GameObject.FindGameObjectWithTag("Player");
        attack = Mode.NONE;
        smackFollowTime = smackFollow;
	}
	
	// Update is called once per frame
	void Update () {

        switch (attack)
        {
            case Mode.THROW:
                break;
            case Mode.SWIPE:
                break;
            case Mode.HOVER:
                smackFollowTime -= Time.deltaTime;
                if(smackFollowTime <= 0)
                {
                    SmackDown();
                }else
                {
                    follow = hunter.transform.position;
                    follow.y = HoverY;
                   transform.position = Vector3.MoveTowards(transform.position, follow,8*Time.deltaTime);
                }
                break;
            case Mode.BLOCK:
                break;
            case Mode.NONE:
                break;
            default:
                break;
        }

        if (isAttacking == true)
        {
            ChargeSmackDown();
            isAttacking = false;
        }
	}


    public void ThrowBall()
    {
        attack = Mode.THROW;
        spawner.Spawn();
    }

    public void Swipe()
    {
        attack = Mode.SWIPE;
        SwipeSequence();
    }

    public void SwipeSequence()
    {
        //TweenCallback CallBack;
        
        Sequence swipeSequence = DOTween.Sequence();
        tempPos = this.transform.position;
        tempRot = this.transform.rotation.eulerAngles;
        swipeSequence.Append(transform.DOMove(startSwipePos.transform.position, durationFirstSectionSwipe, false)).Append(transform.DOPunchPosition(transform.forward*0.1f,durationSecondSectionSwipe,50,0.5f,false)).AppendInterval(SwipeWait).Append(transform.DOMove(endSwipePos.transform.position, durationThirdSectionSwipe, false)).Append(transform.DOMove(tempPos, durationForthSectionSwipe, false)).OnComplete(() =>
        {
            attack = Mode.NONE;
        });
        swipeSequence.Insert(0, transform.DORotate(rotationSwipe, 0.5f));
        swipeSequence.Insert(durationFirstSectionSwipe + durationSecondSectionSwipe + durationThirdSectionSwipe+ SwipeWait, transform.DORotate(tempRot, 0.5f));
        
    }

    public void ChargeSmackDown()
    {
        attack = Mode.CHARGE;
        follow = hunter.transform.position;
        follow.y = HoverY;
        Sequence ChargeSmackDown = DOTween.Sequence();
        tempPos = this.transform.position;
        tempRot = this.transform.rotation.eulerAngles;
        ChargeSmackDown.Append(transform.DOMove(startSmackPos.transform.position, durationFirstSectionChargeSmack, false)).Append(transform.DOMove(follow, durationSecondSectionChargeSmack, false)).OnComplete(() =>
        {
            attack = Mode.HOVER;
        });
        ChargeSmackDown.Insert(durationFirstSectionChargeSmack, transform.DORotate(rotationSmack, durationSecondSectionChargeSmack));
        //SmackDown.Insert(durationFirstSectionSwipe + durationSecondSectionSwipe, transform.DORotate(tempRot, 0.5f));
        //.Append(transform.DOMove(tempPos, durationThirdSectionSmack, false));


    }

    public void SmackDown()
    {
        attack = Mode.SMACK;
        follow = hunter.transform.position;
        follow.y = aboveGroound;
        Sequence SmackDown = DOTween.Sequence();
        SmackDown.Append(transform.DOPunchPosition(transform.forward * 0.3f, durationSecondSectionSwipe, 50, 0.5f, false)).Append(transform.DOMove(follow, durationFirstSectionSmack, false)).Append(transform.DOMove(tempPos, durationSecondSectionSmack, false)).OnComplete(() =>
        {
            attack = Mode.NONE;
            smackFollowTime = smackFollow;
        });
        SmackDown.Insert(durationFirstSectionSmack + durationSecondSectionSmack + durationSecondSectionSwipe, transform.DORotate(tempRot, 0.5f));
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
