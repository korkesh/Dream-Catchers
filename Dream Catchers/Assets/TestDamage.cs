using UnityEngine;
using System.Collections;

public class TestDamage : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Character_Manager.instance.takeDamage(10);

        }
    }
}
