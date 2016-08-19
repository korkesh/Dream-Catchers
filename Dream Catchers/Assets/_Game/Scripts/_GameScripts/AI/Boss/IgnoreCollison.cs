using UnityEngine;
using System.Collections;


public class IgnoreCollison : MonoBehaviour {

    public string TagNameObjectToAvoid;
    public GameObject[] avoids;
    public Collider thisColiider;
    //GameObject avoiding;

	// avoids colliders
	void Awake () {

        if(thisColiider != null)
        {
            avoids = GameObject.FindGameObjectsWithTag(TagNameObjectToAvoid);
            for (int i = 0; i < avoids.Length; i++)
            {
                Collider other = avoids[i].GetComponent<Collider>();
                if (other != null)
                {
                    Physics.IgnoreCollision(thisColiider, other);
                }
            }
        }

	}
	
}
