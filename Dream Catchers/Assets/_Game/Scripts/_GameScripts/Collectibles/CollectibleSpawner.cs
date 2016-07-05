//================================
// Alex
//  spawns collectibles when destroyed needs health
//================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectibleSpawner : MonoBehaviour {

    public GameObject Prefabs;
    public float probabilityOfDrop;
    public string Scene;
    HealthManager HM;
     GameObject spawns;
     //public string name;

	// Use this for initialization
	void Start () {
	
        if(probabilityOfDrop > 1)
        {
            probabilityOfDrop = probabilityOfDrop / 100;
        }

        HM = GetComponent<HealthManager>();
	}

    //Item drop when the object is destroyed
    void OnDestroy()
    {
        float rand;
        rand = Random.value;
        if(rand < probabilityOfDrop && HM != null)
        {
            if(HM.spawns == true)
            {
                spawns = Prefabs;
                //spawns = Instantiate(Prefabs);
                //spawns.name = spawns.name + this.name;
                GameObject t = (GameObject)Instantiate(spawns, this.transform.position, spawns.transform.rotation);
                Items i = t.GetComponent<Items>();
                i.key = i.key + this.gameObject.name + Scene;
                i.checkIfCollected();
                //Destroy(spawns);
            }
            
        }
          
    }
}
