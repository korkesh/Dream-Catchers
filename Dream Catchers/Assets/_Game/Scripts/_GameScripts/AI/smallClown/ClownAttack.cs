using UnityEngine;
using System.Collections;

public class ClownAttack : MonoBehaviour {

    public SphereCollider spCollider;
    public PhysicMaterial physMat;
    public Rigidbody rigidB;
    public ManipulationManager.WORLD_STATE currentState;
    public DamageDealer damage;
    public bool hitBack;
    public float sightRadius;
    public GameObject clown;
    public float timer;

    void Awake()
    {
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            spCollider.material = physMat;
        }
        else
        {
            spCollider.material = null;
        }
        hitBack = false;
    }

	// Use this for initialization
	void Start () {

        currentState = ManipulationManager.instance.currentWorldState;
        spCollider = GetComponent<SphereCollider>();
        damage = GetComponent<DamageDealer>();
        rigidB = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(this.gameObject);
        }

        if( currentState != ManipulationManager.instance.currentWorldState)
        {
            if(ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
            {
                spCollider.material = physMat;
            }
            else
            {
                spCollider.material = null;
            }

            currentState = ManipulationManager.instance.currentWorldState;

        }
	
	}

    void OnCollisionEnter(Collision collision)
    {
        if (currentState == ManipulationManager.WORLD_STATE.NIGHTMARE && collision.gameObject.name != "Launch")
        {
            if(collision.gameObject.tag == "Player")
            {
                damage.DealDamage();
            }

            Debug.Log(collision.gameObject.tag);
            if (collision.gameObject.tag == "Enemy")
            {
                HealthManager Health = collision.gameObject.GetComponent<HealthManager>();
                Debug.Log("himself");
                if(Health != null)
                {
                    Debug.Log("TakeDamage");
                    Health.TakeDamage(damage.Damage);
                }
            }

            Destroy(this.gameObject);

        }
        else if (currentState == ManipulationManager.WORLD_STATE.DREAM)
        {
            if(collision.gameObject.name == clown.name)
            {
                //clown.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

            if(collision.gameObject.tag == "Hammer" && hitBack == false)
            {
                Debug.Log("hit hammer");
               if(EnemyInCone() == true)
               {
                   hitBack = true;
                   Debug.Log("lob");
                  rigidB.velocity = clown.GetComponent<SmallClownAI>().Jump(clown.transform.position + Vector3.up, clown.GetComponent<SmallClownAI>().LuanchAngle, transform);
               }
               else
               {
                   GameObject player = GameObject.FindGameObjectWithTag("Player");
                   Vector3 inFrontofPlayer = player.transform.position + player.transform.forward * 5;
                   rigidB.velocity = clown.GetComponent<SmallClownAI>().Jump(inFrontofPlayer, clown.GetComponent<SmallClownAI>().LuanchAngle, transform);
               }
            }
            else if (collision.gameObject.tag == "Player" && hitBack == false) 
            {
                
                Animator anim = collision.gameObject.GetComponent<Animator>();
                if(anim != null)
                {
                    if (anim.GetBool("StandAttack") == true )
                    {
                        if(EnemyInCone() == true)
                        {
                            hitBack = true;
                            rigidB.velocity = clown.GetComponent<SmallClownAI>().Jump(clown.transform.position + Vector3.up, clown.GetComponent<SmallClownAI>().LuanchAngle, transform);
                        }
                        else
                        {
                            //GameObject player = GameObject.FindGameObjectWithTag("Player");
                            //Vector3 inFrontofPlayer = player.transform.position + player.transform.forward*5;
                            //rigidB.velocity = clown.GetComponent<SmallClownAI>().Jump(inFrontofPlayer, clown.GetComponent<SmallClownAI>().LuanchAngle, transform);
                            Destroy(this.gameObject);
                        }
                    }
                }
            }
        }
    }

    bool EnemyInCone()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 inFrontofPlayer = player.transform.position + player.transform.forward;
        Vector2 infrontNoy = Vector2.zero;
        infrontNoy.x = inFrontofPlayer.x;
        infrontNoy.y = inFrontofPlayer.z;
        Vector2 enemyNoy = Vector2.zero;
        enemyNoy.x = clown.transform.position.x;
        enemyNoy.y = clown.transform.position.z;
        infrontNoy.Normalize();
        enemyNoy.Normalize();
        float angle = Vector2.Dot(infrontNoy, enemyNoy);
        if(angle > (1-sightRadius))
        {
            return true;
        }
        return false;
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (currentState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            Destroy(this.gameObject);
        }
    }

}
