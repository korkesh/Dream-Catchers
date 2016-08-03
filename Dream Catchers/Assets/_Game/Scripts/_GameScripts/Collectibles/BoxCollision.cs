using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxCollision : MonoBehaviour {

    public float groundHeight;
    public List<GameObject> boxesToDestroy;
    public GameObject colliderToDestroy;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        foreach(GameObject g in boxesToDestroy)
        {
            if(g != null)
            {
                return;
            }
        }

        if (transform.position.y <= groundHeight)
        {
            colliderToDestroy.SetActive(false);
        }
    }
}
