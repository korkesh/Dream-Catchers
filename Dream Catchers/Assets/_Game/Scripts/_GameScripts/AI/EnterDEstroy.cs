using UnityEngine;
using System.Collections;

public class EnterDEstroy : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
        if(other.tag == "Player")
        {
            Debug.Log("Destroy");
            Destroy(this.gameObject);
        }
        
    }
}
