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
    public GameObject particlesDream;
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

        //for balls to despawn
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            detroyBall();
            
            
        }

        //switch to a bouncy physics material if in dream
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

    public void detroyBall()
    {
        if (exploded == false && hitBack == false)
        {

                SpawnParticles();
           
        }
        else if(exploded == true)
        {
            Destroy(this.gameObject);
        }
            //remove this if uncomment
    }

    //from online
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
        //if exploded wait before destroying
        if(exploded == true)
        {
            wait -= Time.deltaTime;
            if(wait <= 0)
            {
                Destroy(this.gameObject);
            }
            return;
        }

        //check collisions
        if (currentState == ManipulationManager.WORLD_STATE.NIGHTMARE && collision.gameObject.name != "Launch" && exploded == false)
        {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Hammer")
            {
                damage.DealDamage();
            }

            if (collision.gameObject == clown && Nightmaremessagetosend != "")
            {
                clown.SendMessage(Nightmaremessagetosend, SendMessageOptions.DontRequireReceiver);
            }

            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "JackInTheBox")
            {
                HealthManager Health = collision.gameObject.GetComponent<HealthManager>();
                if(Health != null)
                {
                    Health.TakeDamage(damage.Damage);
                }
            }

            SpawnParticles();
            

        }
        else if (currentState == ManipulationManager.WORLD_STATE.DREAM)
        {


            if (collision.gameObject.tag == "BossHand" || collision.gameObject.tag == "BossHead" || (collision.gameObject.tag == "JackInTheBox" && hitBack == true) || (collision.gameObject == clown && hitBack == true))
             {

                 SpawnParticles();

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
                            SpawnParticles();
                           // Destroy(this.gameObject);
                        }
                    }
                }
            }
        }
    }


    //checks if the enemy is within your sight when u hit the ball back using dot product
    bool EnemyInCone()
    {
        if(clown!= null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 inFrontofPlayer = player.transform.forward; 
            Vector2 infrontNoy = Vector2.zero;
            infrontNoy.x = inFrontofPlayer.x;
            infrontNoy.y = inFrontofPlayer.z;
            Vector2 enemyNoy = Vector2.zero;
            enemyNoy.x = clown.transform.position.x-player.transform.position.x;
            enemyNoy.y = clown.transform.position.z - player.transform.position.z;
            infrontNoy.Normalize();
            enemyNoy.Normalize();
            float angle = Vector2.Dot(infrontNoy, enemyNoy);
            if (angle > (1 - sightRadius/90))
            {
                return true;
            }
        }
        
        return false;
    }

    //hack for bomb state when staying in geometry
    void OnCollisionStay(Collision collisionInfo)
    {
        if (currentState == ManipulationManager.WORLD_STATE.NIGHTMARE && exploded == false)
        {
            GetComponent<AudioSource>().Stop();
            gameObject.SendMessage("PlayAlt");
            SpawnParticles();
            
        }
    }

    //spawn correct particles
    public void SpawnParticles()
    {
        exploded = true;
        GameObject ex = null;
        spCollider.enabled = false;
        rigidB.useGravity = false;
        if (NightmareBall != null)
        {
            Destroy(NightmareBall);
        }
        if (DreamBall != null)
        {
            Destroy(DreamBall);
        }
        if(currentState == ManipulationManager.WORLD_STATE.DREAM)
        {
            if(particlesDream != null)
            {
                ex = (GameObject)Instantiate(particlesDream, this.transform.position, this.transform.rotation);
            }
        }
        else
        {
            if (particles != null)
            {
                ex = (GameObject)Instantiate(particles, this.transform.position, this.transform.rotation);
            }
        }
        //GameObject ex = (GameObject)Instantiate(particlesDream, this.transform.position, this.transform.rotation);
        if(ex != null)
        {
            ex.transform.parent = this.transform;
            ex.SetActive(true);
            timer = 0.5f;
            rigidB.constraints = RigidbodyConstraints.FreezeAll;
        }
        
    }

}
