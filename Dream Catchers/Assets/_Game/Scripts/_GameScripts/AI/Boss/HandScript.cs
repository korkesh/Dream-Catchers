using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HandScript : MonoBehaviour {


    //all modes
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

    public GameObject particles;

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
            default:
                break;
        }
	}

    //spawn the ball
    public void ThrowBall()
    {
        attack = Mode.THROW;
        spawner.Spawn();
    }

    //start the spike
    public void Swipe()
    {
        attack = Mode.SWIPE;
        anim.SetTrigger("RotateToSweep");
        SwipeSequence();
    }

    //swipe sequence
    public void SwipeSequence()
    {
        
        Sequence swipeSequence = DOTween.Sequence();
        tempPos = this.transform.position;
        swipeSequence.Append(transform.DOMove(startSwipePos.transform.position, durationFirstSectionSwipe, false)).AppendInterval(SwipeWait).Append(transform.DOMove(endSwipePos.transform.position, durationForthSectionSwipe, false));
        
    }


    //start the charge smack
    public void ChargeSmackDown()
    {
        attack = Mode.CHARGE;
        tempPos = this.transform.position;
        anim.SetTrigger("RotateToSlam");

    }


    //start smack down
    public void SmackDown()
    {
        attack = Mode.SMACK;
        smackFollowTime = smackFollow;
        anim.SetTrigger("Slam");

    }

    //go to block
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

    }

    //back to resting pos
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



    public void TurnOffCollider()
    {
        Col.enabled = false;
       
    }

    public void TurnOnCollider()
    {
        Col.enabled = true;
    }

    //check hitting the player or dammage trigger

    void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.tag == "Player")
       {
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

    //check hitting the ball
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


    //death particles
    void OnDestroy()
    {
        HealthManager HM = this.GetComponent<HealthManager>();
        if (HM != null)
        {
            if(HM.currentHealth <= 0)
            {
                if(particles != null)
                {
                    particles.SetActive(true);
                    particles.transform.SetParent(null);
                }
            }
        }
    }

}
