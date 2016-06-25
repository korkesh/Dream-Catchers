//================================
// Alex
//  trigger for spawning enemys
//================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour {

    //list of enemys
    public List<GameObject> enemys;

    void Awake()
    {
        //active false
        for (int i = 0; i < enemys.Count; i++)
        {
            if (enemys[i] != null)
            {
                enemys[i].SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //active true
        if(other.gameObject.tag == "Player")
        {
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] != null)
                {
                    enemys[i].SetActive(true);
                }
            }
        }
       
    }
}
