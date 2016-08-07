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
        UNBLOCK,
        CHARGE,
        NONE
    }
   public  Mode attack;
   public Collider Col;

    public int Health;
    public BallSpawner spawner;
    public GameObject startSwipePos;
    public GameObject endSwipePos;
    public GameObject startSmackPos;
    public GameObject hunter;
    public GameObject blockSpot;
    public Vector3 rotationSwipe;
    public Vector3 rotationSmack;
    public Vector3 rotationBlock;
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
   public  Vector3 tempPos;
    public Vector3 tempRot;
    public bool isAttacking;
    public DamageDealer Damage;
    public float smackFollow;
    float smackFollowTime;
    Vector3 follow;
    public GameObject HoverSphere;

    public bool returnSpot;

    public Animator anim;

	// Use this for initialization
	void Start () {
        hunter = GameObject.FindGameObjectWithTag("Player");
        attack = Mode.NONE;
        smackFollowTime = smackFollow;
        anim.applyRootMotion = false;
	}
	
	// Update is called once per frame
	void Update () {

        switch (attack)
        {
            case Mode.THROW:
                if(spawner.currentAttack == null)
                {
                    attack = Mode.NONE;
                }
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
                    follow = HoverSphere.transform.position;
                    follow.y = HoverY;
                    //HoverSphere.transform.position
                    transform.position = Vector3.MoveTowards(this.transform.position, follow, 9 * Time.deltaTime);
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
            //ThrowBall();
            //Block();
            ChargeSmackDown();
            //Swipe();
            isAttacking = false;
        }else if (returnSpot == true)
        {
            BlockReturn();
            returnSpot = false;
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
        anim.SetTrigger("RotateToSweep");
        SwipeSequence();
    }

    public void SwipeSequence()
    {
        //TweenCallback CallBack;
        
        Sequence swipeSequence = DOTween.Sequence();
        tempPos = this.transform.position;

        //tempRot = this.transform.rotation.eulerAngles;
        //swipeSequence.Append(transform.DOMove(startSwipePos.transform.position, durationFirstSectionSwipe, false)).Append(transform.DOPunchPosition(transform.forward*0.1f,durationSecondSectionSwipe,50,0.5f,false)).AppendInterval(SwipeWait).Append(transform.DOMove(endSwipePos.transform.position, durationThirdSectionSwipe, false)).Append(transform.DOMove(tempPos, durationForthSectionSwipe, false)).OnComplete(() =>
        //{
        //    attack = Mode.NONE;
        //});
        //swipeSequence.Insert(0, transform.DORotate(rotationSwipe, 0.5f));
        //swipeSequence.Insert(durationFirstSectionSwipe + durationSecondSectionSwipe + durationThirdSectionSwipe+ SwipeWait, transform.DORotate(tempRot, 0.5f));

        swipeSequence.Append(transform.DOMove(startSwipePos.transform.position, durationFirstSectionSwipe, false)).AppendInterval(SwipeWait).Append(transform.DOMove(endSwipePos.transform.position, durationForthSectionSwipe, false));
        
    }

    public void ChargeSmackDown()
    {
        attack = Mode.CHARGE;
        tempPos = this.transform.position;
        anim.SetTrigger("RotateToSlam");



        //follow = hunter.transform.position;
        //follow.y = HoverY;
        //Sequence ChargeSmackDown = DOTween.Sequence();
        //tempPos = this.transform.position;
        //tempRot = this.transform.rotation.eulerAngles;
        //ChargeSmackDown.Append(transform.DOMove(startSmackPos.transform.position, durationFirstSectionChargeSmack, false)).Append(transform.DOMove(follow, durationSecondSectionChargeSmack, false)).OnComplete(() =>
        //{
        //    attack = Mode.HOVER;
        //});
        //ChargeSmackDown.Insert(durationFirstSectionChargeSmack, transform.DORotate(rotationSmack, durationSecondSectionChargeSmack));


        //SmackDown.Insert(durationFirstSectionSwipe + durationSecondSectionSwipe, transform.DORotate(tempRot, 0.5f));
        //.Append(transform.DOMove(tempPos, durationThirdSectionSmack, false));


    }

    public void SmackDown()
    {
        attack = Mode.SMACK;
        smackFollowTime = smackFollow;
        anim.SetTrigger("Slam");


        //follow = hunter.transform.position;
        //follow.y = aboveGroound;
        //Sequence SmackDown = DOTween.Sequence();
        //SmackDown.Append(transform.DOPunchPosition(transform.forward * 0.3f, durationSecondSectionSwipe, 50, 0.5f, false)).Append(transform.DOMove(follow, durationFirstSectionSmack, false)).Append(transform.DOMove(tempPos, durationSecondSectionSmack, false)).OnComplete(() =>
        //{
        //    attack = Mode.NONE;
        //    smackFollowTime = smackFollow;
        //});
        //SmackDown.Insert(durationFirstSectionSmack + durationSecondSectionSmack + durationSecondSectionSwipe, transform.DORotate(tempRot, 0.5f));
    }


    public void Block()
    {
        attack = Mode.BLOCK;
        if(Col != null)
        {
            Col.isTrigger = false;
        }
        tempPos = this.transform.position;
        tempRot = this.transform.rotation.eulerAngles;
        Sequence BlockIt = DOTween.Sequence();
        BlockIt.Append(transform.DOMove(blockSpot.transform.position, 0.5f, false));
        BlockIt.Insert(0, transform.DORotate(rotationBlock, 0.5f));
        
        //Sequence BlockIt = DOTween.Sequence();
        //BlockIt.Append(transform.DOMove(blockSpot.transform.position, 0.5f, false));
        //BlockIt.Insert(0, transform.DORotate(rotationBlock, 0.5f));
    }

    public void BlockReturn()
    {
        attack = Mode.UNBLOCK;
        Sequence BlockItReturn = DOTween.Sequence();
        BlockItReturn.Append(transform.DOMove(tempPos, 0.5f, false)).OnComplete(() =>
        {
            attack = Mode.NONE;
        }); 
        BlockItReturn.Insert(0, transform.DORotate(tempRot, 0.5f));
        if (Col != null)
        {
            Col.isTrigger = true;
        }
    }

    public void ReturnToTempPos()
    {
        Sequence Return = DOTween.Sequence();
        Return.Append(transform.DOMove(tempPos, 1.5f, false)).OnComplete(() =>
        {
            attack = Mode.NONE;

        }); 
    }

    public void HoverStart()
    {
        attack = Mode.HOVER;
    }

    public void SlowDownAnim()
    {
        anim.speed = 0.5f;
        
    }

    public void SpeedUpAnim()
    {
        anim.speed = 1.0f;
        
    }

    void OnDestroy()
    {
        
    }

    public void TurnOffCollider()
    {
        Col.enabled = false;
       
    }

    public void TurnOnCollider()
    {
        Col.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit " + other.gameObject.name);
       if(other.gameObject.tag == "Player")
       {
           Debug.Log("hunter");
           //isAttacking == true && 
           Damage.DealDamage();
       }else if(other.gameObject.name == "DamageTrigger" && attack == Mode.SMACK && ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
       {
           HealthManager HM = this.GetComponent<HealthManager>();
           if (HM != null)
           {
               HM.TakeDamage(2);
           }
       }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Ball")
        {
            if(ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
            {
                HealthManager HM = this.GetComponent<HealthManager>();
                if(HM != null)
                {
                    HM.TakeDamage(Damage.Damage);
                }
            }

            BlockReturn();
        }
    }

}
