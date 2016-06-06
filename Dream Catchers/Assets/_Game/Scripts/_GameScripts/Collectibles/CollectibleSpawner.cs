using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectibleSpawner : MonoBehaviour {

    public List<GameObject> Prefabs;
    public float probabilityOfDrop;

	// Use this for initialization
	void Start () {
	
        if(probabilityOfDrop > 1)
        {
            probabilityOfDrop = probabilityOfDrop / 100;
        }
	}

    //Item drop when the object is destroyed
    void OnDestroy()
    {
        float rand;
        for(int i = 0; i < Prefabs.Count; i++)
        {
            rand = Random.value;
            if(rand < probabilityOfDrop)
            {
                Instantiate(Prefabs[i], this.transform.position, this.transform.rotation);
            }
           
        }
    }
}
