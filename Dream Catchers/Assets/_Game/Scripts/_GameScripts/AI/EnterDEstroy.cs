using UnityEngine;
using System.Collections;

public class EnterDEstroy : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Destroy(this.gameObject);
        }
        
    }
}
