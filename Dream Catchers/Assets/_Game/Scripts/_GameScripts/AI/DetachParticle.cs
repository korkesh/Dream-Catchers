using UnityEngine;
using System.Collections;

public class DetachParticle : MonoBehaviour {

    public GameObject detach;

    void OnDestroy()
    {
        detach.transform.parent = null;
        detach.SetActive(true);
    }
}
