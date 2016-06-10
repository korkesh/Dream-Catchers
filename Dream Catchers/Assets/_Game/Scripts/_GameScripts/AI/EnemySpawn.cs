using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour {

    public List<GameObject> enemys;

    void Awake()
    {
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
        Debug.Log("Fun");
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
