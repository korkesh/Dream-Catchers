using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectibleSpawner : MonoBehaviour {

    public GameObject Prefabs;
    public float probabilityOfDrop;
     GameObject spawns;
     //public string name;

	// Use this for initialization
	void Start () {
	
        if(probabilityOfDrop > 1)
        {
            probabilityOfDrop = probabilityOfDrop / 100;
        }


        spawns = Instantiate(Prefabs);
        if(name != "" || name == null)
        {
            spawns.name = this.name;
        }
        
        
	}

    //Item drop when the object is destroyed
    void OnDestroy()
    {
        float rand;
        rand = Random.value;
        if(rand < probabilityOfDrop)
        {
            Instantiate(spawns, this.transform.position, this.transform.rotation);
        }
           
        
    }
}
