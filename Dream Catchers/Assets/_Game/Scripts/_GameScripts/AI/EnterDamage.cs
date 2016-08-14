//================================
// Alex
//  damage triggers for damage floors.
//================================

using UnityEngine;
using System.Collections;

public class EnterDamage : MonoBehaviour {

    public DamageDealer d;
    public float Timer; //Invincibility Frames
    [HideInInspector]
    public float time;
    bool hit;

    void Start()
    {
        time = Timer;
        hit = false;
    }

    //initial enter deal damage
    void OnTriggerEnter(Collider collision)
    {
        if(ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            if (collision.gameObject.tag == "Player" && hit == false)
            {
                d.DealDamage();
                hit = true;
            }
            else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "BossHand")
            {
                HealthManager h = collision.GetComponent<HealthManager>();
                if (h != null)
                {
                    h.TakeDamage(d.Damage);
                }
                else
                {
                    h = GetComponentInParent<HealthManager>();
                    if (h != null)
                    {
                        h.TakeDamage(d.Damage);
                    }
                }
            }
        }
        
    }

    //after some time deal damage 
    void OnTriggerStay(Collider other)
    {

        if (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            if (other.gameObject.tag == "Player")
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    d.DealDamage();
                    time = Timer;
                }
            }
        }
        

    }


    void OnTriggerExit(Collider other)
    {
        time = Timer;
        hit = false;
    }
}
