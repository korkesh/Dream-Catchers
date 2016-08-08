//================================
// Alex
//  clown ball attack. Explodes and checks if character knocks it back
//================================
using UnityEngine;
using System.Collections;

public class ClownAttack : MonoBehaviour {


    //================================
    // Variables
    //================================
    public SphereCollider spCollider;
    public PhysicMaterial physMat;
    public Rigidbody rigidB;
    public ManipulationManager.WORLD_STATE currentState;
    public DamageDealer damage;
    public bool hitBack;
    public float sightRadius;
    public GameObject clown;
    public float timer;
    public  bool exploded;
    public float wait;
    public GameObject DreamBall;
    public GameObject NightmareBall;
    public GameObject particles;
    public float LaunchAngle;
    public string Nightmaremessagetosend;
    public string Hammermessagetosend;

    //================================
    // Methods
    //================================

    //-----------------
    // Initialization
    //-----------------

    void Awake()
    {

        exploded = false;
        //is bouncy ball or bomb
        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            spCollider.material = physMat;
        }
        else
        {
            spCollider.material = null;
        }
        //hasnt been hit back yet
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
        if (NightmareBall == null)
        {
            //Destroy(this.gameObject);
            //Destroying the gameobject makes the timing on new ball spawns less weird, but it destroys the particle effect as well
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

    //checksa if the bomb should explode, deal damage or bouncy back
    void OnCollisionEnter(Collision collision)
    {
        if(exploded == true)
        {
            wait -= Time.deltaTime;
            if(wait <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        if (currentState == ManipulationManager.WORLD_STATE.NIGHTMARE && collision.gameObject.name != "Launch")
        {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Hammer")
            {
                damage.DealDamage();
            }

            if (collision.gameObject == clown && Nightmaremessagetosend != "")
            {
                clown.SendMessage(Nightmaremessagetosend, SendMessageOptions.DontRequireReceiver);
            }

            //Debug.Log(collision.gameObject.tag);
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "JackInTheBox")
            {
                HealthManager Health = collision.gameObject.GetComponent<HealthManager>();
                if(Health != null)
                {
                    //Debug.Log("TakeDamage");
                    Health.TakeDamage(damage.Damage);
                }
            }

            spCollider.enabled = false;
            rigidB.useGravity = false;
            exploded = true;
            Destroy(NightmareBall);
            Destroy(DreamBall);
            GameObject ex = (GameObject)Instantiate(particles, this.transform.position, this.transform.rotation);
            ex.transform.parent = this.transform;
            ex.SetActive(true);

        }
        else if (currentState == ManipulationManager.WORLD_STATE.DREAM)
        {
            if(collision.gameObject.name == clown.name)
            {
                //clown.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

            if (collision.gameObject.tag == "BossHand" || collision.gameObject.tag == "BossHead")
             {

                 spCollider.enabled = false;
                 rigidB.useGravity = false;
                 exploded = true;
                 Destroy(NightmareBall);
                 Destroy(DreamBall);
                 GameObject ex = (GameObject)Instantiate(particles, this.transform.position, this.transform.rotation);
                 ex.transform.parent = this.transform;
                 ex.SetActive(true);
             }

            if(collision.gameObject.tag == "Hammer" && hitBack == false)
            {
                
               if(EnemyInCone() == true)
               {
                  hitBack = true;
                  rigidB.velocity = Jump(clown.transform.position + Vector3.up, LaunchAngle, transform);
                   if(Hammermessagetosend != "")
                   {
                       clown.SendMessageUpwards(Hammermessagetosend, this.gameObject, SendMessageOptions.DontRequireReceiver);
                   }
                  
               }
               else
               {
                   GameObject player = GameObject.FindGameObjectWithTag("Player");
                   Vector3 inFrontofPlayer = player.transform.position + player.transform.forward * 5;
                   rigidB.velocity = Jump(inFrontofPlayer, LaunchAngle, transform);
               }

              
            }
            else if (collision.gameObject.tag == "Player" && hitBack == false) 
            {
                
                Animator anim = collision.gameObject.GetComponent<Animator>();
                if(anim != null)
                {
                    if (anim.GetBool("SideSwing") == true )
                    {
                        if(EnemyInCone() == true)
                        {
                            hitBack = true;
                            rigidB.velocity = Jump(clown.transform.position + Vector3.up, LaunchAngle, transform);
                            if (Hammermessagetosend != "")
                            {
                                clown.SendMessageUpwards(Hammermessagetosend, this.gameObject, SendMessageOptions.DontRequireReceiver);
                            }
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


    //checks if the enemy is within your sight when u hit the ball back using dot product
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

    //hack for bomb state when staying in geometry
    void OnCollisionStay(Collision collisionInfo)
    {
        if (currentState == ManipulationManager.WORLD_STATE.NIGHTMARE && exploded == false)
        {
            //Destroy(this.gameObject);
            spCollider.enabled = false;
            rigidB.useGravity = false;
            exploded = true;
            Destroy(NightmareBall);
            Destroy(DreamBall);
            GameObject ex = (GameObject)Instantiate(particles, this.transform.position, this.transform.rotation);
            ex.transform.parent = this.transform;
            ex.SetActive(true);
        }
    }

}
