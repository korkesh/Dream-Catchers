using UnityEngine;
using System.Collections;


public class DestroyObject : MonoBehaviour {

    public GameObject[] toDestroy;

    public void DestroyThisObject()
    {
        foreach (GameObject obj in toDestroy)
        {
            Destroy(obj);
        }
    }
}
