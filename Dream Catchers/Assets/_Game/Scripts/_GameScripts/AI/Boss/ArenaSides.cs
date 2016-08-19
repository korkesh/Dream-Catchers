using UnityEngine;
using System.Collections;

public class ArenaSides : MonoBehaviour {

    public GameObject Hunter;
    public bool rightSide;
    public BossClown Bc;

    //collider to check which side the areana hunter is on
    void OnTriggerEnter(Collider other)
    {
       if(other.gameObject == Hunter && Bc != null)
       {
           Bc.hunterOnRight = rightSide;
       }
    }
}
