using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {

    public GameObject SplinterPrefab;

    public void explosion()
    {
        Instantiate(SplinterPrefab, transform.position, transform.rotation);
    }

}
