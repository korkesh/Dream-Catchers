using UnityEngine;
using System.Collections;

public class IgnoreColliders : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Blocker")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }

    }
}
