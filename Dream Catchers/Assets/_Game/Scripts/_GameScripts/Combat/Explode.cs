using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {

    public GameObject SplinterPrefab;

    void OnDestroy()
    {
        Instantiate(SplinterPrefab, transform.position, transform.rotation);
    }

}
