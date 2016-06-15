using UnityEngine;
using System.Collections;

public class EnterDamage : MonoBehaviour {

    public DamageDealer d;
    public float Timer; //Invincibility Frames
    public float time;


    //void OnTriggerEnter(Collider collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        time = 0;
    //    }
    //}

    void OnTriggerEnter(Collider collision)
    {
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
