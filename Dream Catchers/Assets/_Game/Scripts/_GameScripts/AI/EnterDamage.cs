using UnityEngine;
using System.Collections;

public class EnterDamage : MonoBehaviour {

    public DamageDealer d;
    public float Timer;
    float time;

    void Awake()
    {

        Debug.Log("FUCKING SHOW UP");
    }
    

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.tag == "Player")
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                d.DealDamage();
                time = Timer;
            }
        }

    }



    //void OnCollisionExit(Collision collisionInfo)
    //{
    //    Debug.Log("EXit");
    //    if (collisionInfo.gameObject.tag == "Player")
    //    {
    //        time = Timer;
    //    }
    //}
}
