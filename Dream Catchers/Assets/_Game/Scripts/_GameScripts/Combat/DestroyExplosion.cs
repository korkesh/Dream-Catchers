using UnityEngine;
using System.Collections;

public class DestroyExplosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>(), GetComponent<Collider>());
        StartCoroutine(DestroySplinters());

    }

    // Update is called once per frame
    void Update () {
	
	}

    public IEnumerator DestroySplinters()
    {
        yield return new WaitForSeconds(4.0f);
        Destroy(gameObject);
    }
}
