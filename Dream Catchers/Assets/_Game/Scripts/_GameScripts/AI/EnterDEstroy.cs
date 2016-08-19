using UnityEngine;
using System.Collections;

public class EnterDestroy : MonoBehaviour {

    //trigger gets destroyed if hunter enters
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Destroy(this.gameObject);
        }
        
    }
}
