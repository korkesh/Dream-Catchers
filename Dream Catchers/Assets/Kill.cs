using UnityEngine;
using System.Collections;

public class Kill : MonoBehaviour {

    void OnCollisionEnter(Collision other)
    {
        Destroy(other.gameObject);
    }
}
