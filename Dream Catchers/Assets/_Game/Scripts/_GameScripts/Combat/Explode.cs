using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {

    public GameObject splintersPrefab;

    void OnDestroy()
    {
        Instantiate(splintersPrefab, transform.position, transform.rotation);
    }

}
