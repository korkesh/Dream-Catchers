using UnityEngine;
using System.Collections;

public class DestroyParticles : MonoBehaviour {

     public ParticleSystem PS;
	
	// Update is called once per frame
    //void Update () {
	
    //    if(PS.IsAlive() == false)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //}


    void Awake()
     {
         Destroy(this.gameObject, PS.startLifetime);
     }
}
