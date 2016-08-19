using UnityEngine;
using System.Collections;

public class DestroyParticles : MonoBehaviour {

     public ParticleSystem PS;
	
    // destroy particles system after the lifetime is up
    void Awake()
     {
         Destroy(this.gameObject, PS.startLifetime);
     }
}
